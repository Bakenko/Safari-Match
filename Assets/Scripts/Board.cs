using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

//01 - Creando la cuadricula
//02 - Ajustando la camara
//03 - Sistema de coordenadas para instanciar las piezas
//04 - Instanciando las piezas en la cuadrícula
//06 - Intercambiando las piezas de lugar
//07 - Permitiendo solo ciertos tipos de movimientos

public class Board : MonoBehaviour
{
    public int width;               //01.1 - ancho de la cuadricula
    public int height;              //01.2 - alto de la cuadricula
    public GameObject tileObject;   //01.3 - objeto por defecto para cada espacio de la caudricula (empty)

    public float cameraSizeOffset;  //02.2 - agregar un numero al tamaño ortografico final de la camara (se agrega a 02.1.6)
    public float cameraVerticalOffset;  //02.3 - permite agregar un valor a la posicion en vertical de la camara (se agrega a 02.1.3)

    public GameObject[] availablePieces;    //04.7 - son las piezas que van a estar disponibles para ser creadas en la cuadricula

    Tile[,] Tiles;      //06.1 - este arrey de 2 dimensiones sera para guardar todos los espacios de la cuadricula(tiles)
    Piece[,] Pieces;    //06.2 - este arrey de 2 dimensiones sera para guardar todas las piezas

    Tile startTile;     //06.10 - espacio inicial(iniciamos movimiento)
    Tile endTile;       //06.11 - espacion final(terminamos movimiento)

    // Start is called before the first frame update
    void Start()
    {
        //06.13 - inicializamos ambos arreys
        Tiles = new Tile[width,height];     // se inicializa 06.10
        Pieces = new Piece[width, height];  // se inicializa 06.11

        SetupBoard();               //se llama la funcion encargada de crear la cuadricula
        PositionCamera();           //se llama la funcion encargada de ajustar la camara
        SetupPieces();              //se llama la funcion encargada poner las piezas en cada una de las posiciones de la cuadricula
    }

    private void SetupPieces()      //04.8 - funcion encargada de poner las piezas en cada una de las posiciones de la cuadricula
    {
        for (int x = 0; x < width; x++)  //04.8.1 - ancho de la cuadricula
        {
            for (int y = 0; y < height; y++) //04.8.1.1 - alto de la cuadricula
            {
                var selectedPiece = availablePieces[UnityEngine.Random.Range(0, availablePieces.Length)];   //04.8.1.1.2 - selecciona de manera aleatoria una de las piezas
                var o = Instantiate(selectedPiece, new Vector3(x, y, -5), Quaternion.identity);    //04.8.1.1.3 - creamos nuestros objetos
                o.transform.parent = transform;     //04.8.1.1.4 - hacer que el padre del objeto sea la board
                // o.GetComponent<Piece>()?.Setup(x, y, this);  //04.8.1.1.5 - para tener acceso al componente de tipo piece y llamar la funcion Setup
                Pieces[x, y] = o.GetComponent<Piece>(); //06.5 - se guarda la referencaia al componente de Piece que se acaba de crear dentro del arrey de 2 dimensiones de Pieces
                Pieces[x, y].Setup(x, y, this);     //06.6 - se accede al componente que acabamos de guardar
            }
        }
    }

    private void PositionCamera()   //02.1 - funcion encargada de ajustar la camara
    {
        float nexPosX = (float)width / 2f;   //02.1.1 - ancho de la camara
        float nexPosY = (float)height / 2f;  //02.1.2 - alto de la camara

        Camera.main.transform.position = new Vector3(nexPosX -0.5f, nexPosY -0.5f + cameraVerticalOffset, -10f);   //02.1.3 - posicionamiento de la camara

        float horizontal = width + 1;   //02.1.4 - obtener el tamñano ortografico para la dimension horizontal
        float vertical = (height / 2) + 1;  //02.1.5 - obtener el tamñano ortografico para la dimension vertical

        Camera.main.orthographicSize = horizontal > vertical ? horizontal + cameraSizeOffset : vertical + cameraSizeOffset;   //02.1.6 - se pide que escoja el tamaño mayor para el tamaño ortografico de la camara
    }

    private void SetupBoard()       //01.4 - funcion encargada de crear la cuadricula
    {
        for(int x = 0; x < width; x++)  //01.4.1 - ancho de la cuadricula
        {
            for(int y = 0; y < height; y++) //01.4.1.1 - alto de la cuadricula
            {
                var o = Instantiate(tileObject, new Vector3(x, y, -5), Quaternion.identity);    //01.4.1.1.2 - creamos nuestros objetos
                o.transform.parent = transform;     //01.4.1.1.3 - hacer que el padre del objeto sea la board
                // o.GetComponent<Tile>()?.Setup(x, y, this);  //03.5 - para tener acceso al componente de tipo tile y llamar la funcion Setup
                Tiles[x, y] = o.GetComponent<Tile>(); //06.3 - se guarda la referencaia al componente de Tile que se acaba de crear dentro del arrey de 2 dimensiones de Tiles
                Tiles[x, y]?.Setup(x, y, this);     //06.4 - se accede al componente que acabamos de guardar
            }
        }
    }
    
    public void TileDown(Tile tile_)      //06.7 - Funcion para cuando clickeamos
    {
        startTile = tile_;  //06.7.1 - asignar la tile inicial
    }

    public void TileOver(Tile tile_)      //06.8 - Funcion para cuando arrastro el mouse sobre otro espacion de la cuadricula
    {
        endTile = tile_;    //06.8.1 - asignar la tile del final  
    }

    public void TileUp(Tile tile_)      //06.9 - Funcion para cuando levanto el click
    {
        if(startTile != null && endTile != null && IsCloseTo(startTile, endTile))    //06.9.1 - verifica que tengamos un starTile y un endTile
        {
            SwapTiles();    //se llama la funcion encargada de actializar la funcion del sistema de coordenadas [,] y de llamar la funcion de Move
        }
        //06.9.2 - se reinician los valores despues del movimiento
        startTile = null;
        endTile = null;
    }

    private void SwapTiles()    //06.10 - actializa la funcion del sistema de coordenadas [,] y de llamar la funcion de Move
    {
        var StartPiece = Pieces[startTile.x, startTile.y];  //06.10.1 - referencia a la pieza inicial
        var EndPiece = Pieces[endTile.x, endTile.y];        //06.10.2 - referencia a la pieza final

        StartPiece.Move(endTile.x, endTile.y);      //06.10.3 - mueve la pieza inicial a la posicion final
        EndPiece.Move(startTile.x, startTile.y);    //06.10.4 - mueve la pieza final a la posicion inicial

        //06.10.5 - actualiza el sistema de coordenadas de las piezas que se movieron
        Pieces[startTile.x, startTile.y] = EndPiece;
        Pieces[endTile.x, endTile.y] = StartPiece;
    }

    public bool IsCloseTo(Tile start, Tile end)     //07.1 - esta funcion retorna un valor booleano y se agrega a 06.9.1
    {
        if(Math.Abs((start.x-end.x))==1 && start.y == end.y){       //07.1.1 - verificacion horizontal
            return true;
        }
        if (Math.Abs((start.y - end.y)) == 1 && start.x == end.x)   //07.1.2 - verificacion vertical
        {
            return true;
        }
        return false;
    }
}
