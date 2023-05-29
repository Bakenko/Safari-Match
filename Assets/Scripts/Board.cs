using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;      //08.3 - se agrega para poder usar la funcion "union"
using UnityEngine;

//01 - Creando la cuadricula
//02 - Ajustando la camara
//03 - Sistema de coordenadas para instanciar las piezas
//04 - Instanciando las piezas en la cuadrícula
//06 - Intercambiando las piezas de lugar
//07 - Permitiendo solo ciertos tipos de movimientos
//08 - Creando las funciones del match 3
//09 - Usando el match 3 en nuestro juego
//10 - Eliminando los matches por defecto
//11 - Colapsando las piezas

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

    bool swappingPieces = false;    //09.1 - sirve para evitar que tratemos de hacer o buscar dos matches al mismo tiempo o mover dos piezas de seguido

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
        int maxIterations = 50;     //10.5 - agragamos las variables para iterar la piezas
        int currentIteration = 0;   //10.6 - agragamos las variables para iterar las piezas

        for (int x = 0; x < width; x++)  //04.8.1 - ancho de la cuadricula
        {
            for (int y = 0; y < height; y++) //04.8.1.1 - alto de la cuadricula
            {
                //10.1 - se requiere una funcion aparte para crear las piezas por lo que se elimina de aqui
                /*var selectedPiece = availablePieces[UnityEngine.Random.Range(0, availablePieces.Length)];   //04.8.1.1.2 - selecciona de manera aleatoria una de las piezas
                var o = Instantiate(selectedPiece, new Vector3(x, y, -5), Quaternion.identity);    //04.8.1.1.3 - creamos nuestros objetos
                o.transform.parent = transform;     //04.8.1.1.4 - hacer que el padre del objeto sea la board
                // o.GetComponent<Piece>()?.Setup(x, y, this);  //04.8.1.1.5 - para tener acceso al componente de tipo piece y llamar la funcion Setup
                Pieces[x, y] = o.GetComponent<Piece>(); //06.5 - se guarda la referencaia al componente de Piece que se acaba de crear dentro del arrey de 2 dimensiones de Pieces
                Pieces[x, y].Setup(x, y, this);     //06.6 - se accede al componente que acabamos de guardar*/

                currentIteration = 0;

                var newPiece = CreatePieceAt(x, y);     //10.3 - llamamos la funcion para crear las piezas

                while (HasPreviousMatches(x, y))     //10.7 - despues de crear la pieza (10.3) debemos verificar si tiene previous matches
                {
                    ClearPieceAt(x, y);     //10.7.1 - se llama la funcion para destruir la pieza
                    newPiece = CreatePieceAt(x, y);     //10.7.2 - se crea una nueva pieza
                    currentIteration++;     //10.7.3 - hace que las iteraciones aumenten
                    if (currentIteration > maxIterations)       //10.7.4 - si iteracion actual es mayor a la catidad maxima de iteraciones deberiamos romper el while
                    {
                        break;      //10.7.4.1 - se rompe el while loop
                    }
                }
            }
        }
    }

    private void ClearPieceAt(int x, int y)     //10.8 - funcion para borra la pieza del previous matches
    {
        var pieceToClear = Pieces[x, y];        //10.8.1 - obtenemos la referencia
        Destroy(pieceToClear.gameObject);      //10.8.2 - destruimos el gameobject
        Pieces[x, y] = null;                   //10.8.3 - actualizamos el arrey de dos dimensiones de piezas

    }

    private Piece CreatePieceAt(int x, int y)       //10.2 - se crea una funcion nueva para crear las piezas
    {
        var selectedPiece = availablePieces[UnityEngine.Random.Range(0, availablePieces.Length)];   //04.8.1.1.2 - selecciona de manera aleatoria una de las piezas
        var o = Instantiate(selectedPiece, new Vector3(x, y, -5), Quaternion.identity);    //04.8.1.1.3 - creamos nuestros objetos
        o.transform.parent = transform;     //04.8.1.1.4 - hacer que el padre del objeto sea la board
        Pieces[x, y] = o.GetComponent<Piece>(); //06.5 - se guarda la referencaia al componente de Piece que se acaba de crear dentro del arrey de 2 dimensiones de Pieces
        Pieces[x, y].Setup(x, y, this);     //06.6 - se accede al componente que acabamos de guardar
        return Pieces[x, y];       //10.2.1 - retornamos la pieza que esta en esa posicion "x" y "y"
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
        for (int x = 0; x < width; x++)  //01.4.1 - ancho de la cuadricula
        {
            for (int y = 0; y < height; y++) //01.4.1.1 - alto de la cuadricula
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
        if (startTile != null && endTile != null && IsCloseTo(startTile, endTile))    //06.9.1 - verifica que tengamos un starTile y un endTile
        {
            //SwapTiles();    //se llama la funcion encargada de actializar la funcion del sistema de coordenadas [,] y de llamar la funcion de Move
            StartCoroutine(SwapTiles());       //cambiamos la forma de de llamar ya que ahora es una coroutina
        }
        //06.9.2 - se reinician los valores despues del movimiento
        //startTile = null; //09 se lleva a SwapTiles a 09.2.8
        //endTile = null;   //09 se lleva a SwapTiles a 09.2.8
    }

    //private void SwapTiles()    //06.10 - actializa la funcion del sistema de coordenadas [,] y de llamar la funcion de Move
    IEnumerator SwapTiles()     //09.2 - la cambiamos porque ahora necesitamos que mueva las piezas y espere a que las piezas terminen de moverse, busque los matches y envie el resultado, si no fuera IEnumerator todo pasaria instantaneamente y el jugador no podria verlo
    {
        var StarPiece = Pieces[startTile.x, startTile.y];  //06.10.1 - referencia a la pieza inicial
        var EndPiece = Pieces[endTile.x, endTile.y];        //06.10.2 - referencia a la pieza final

        StarPiece.Move(endTile.x, endTile.y);      //06.10.3 - mueve la pieza inicial a la posicion final
        EndPiece.Move(startTile.x, startTile.y);    //06.10.4 - mueve la pieza final a la posicion inicial

        //06.10.5 - actualiza el sistema de coordenadas de las piezas que se movieron
        Pieces[startTile.x, startTile.y] = EndPiece;
        Pieces[endTile.x, endTile.y] = StarPiece;

        yield return new WaitForSeconds(0.6f);      //09.2.1 - despues de ejecutarse lo anterior esperar 0.6 segundos para continuar

        //11.4 - eliminamos la variable foundMatches ya no se va a necesitar (09.2.2)
        //bool foundMatch = false;       //09.2.2 - sirve para marcar si llegamos a encontrar matches
        var startMatches = GetMatchByPiece(startTile.x, startTile.y, 3);    //09.2.3 - para encontar los matches de nuestra pieza inicial
        var endMatches = GetMatchByPiece(endTile.x, endTile.y, 3);    //09.2.4 - para encontar los matches de nuestra pieza final

        var allMatches = startMatches.Union(endMatches).ToList();       //11.2 - para guardar todos los matches conseguidos y luego eliminarlos con la funcion "ClearPieces"


        //11.1 - eliminamos ambos ForEach para crear una funcion nueva que borre los matches
        /*startMatches.ForEach(piece =>       //09.2.5 - para marcar cada match que encontramos como true, la destruimos y actualizamos el sitema de coordenadas
        {
            foundMatch = true;      //09.2.5.1 - marca el match como true
            //Pieces[piece.x, piece.y] = null;        //09.2.5.2 - actualizamos el arrey de dos dimensiones de piezas
            //Destroy(piece.gameObject);      //09.2.5.3 - destruimos el gameobjecto
            ClearPieceAt(piece.x, piece.y);     //10.9 - reemplazamos las dos lineas anterioes (09.2.5.2 y 09.2.5.3) por esta funcion para evitar el codigo repetido
        });

        endMatches.ForEach(piece =>       //09.2.6 - para marcar cada match que encontramos como true, la destruimos y actualizamos el sitema de coordenadas
        {
            foundMatch = true;      //09.2.6.1 - marca el match como true
            //Pieces[piece.x, piece.y] = null;        //09.2.6.2 - actualizamos el arrey de dos dimensiones de piezas
            //Destroy(piece.gameObject);      //09.2.6.3 - destruimos el gameobjecto
            ClearPieceAt(piece.x, piece.y);     //10.9 - reemplazamos las dos lineas anterioes (09.2.6.2 y 09.2.6.3) por esta funcion para evitar el codigo repetido
        });*/

        //if (!foundMatch)    //09.2.7 - si no encontramos ningun foundmatch reiniciamos las piezas que se movieron
        if (allMatches.Count == 0)   //11.3 - cambiamos la variable "foundMatch" por "allMatches"
        {
            StarPiece.Move(startTile.x, startTile.y);          //09.2.7.1 - movemos la pieza de inicio a la posicion inicial
            EndPiece.Move(endTile.x, endTile.y);                //09.2.7.2 - movemos la pieza final a la posicion final
            Pieces[startTile.x, startTile.y] = StarPiece;      //09.2.7.3 - reiniciamos el cambio que hicimos en nuestro arrey de piezas
            Pieces[endTile.x, endTile.y] = EndPiece;           //09.2.7.4 - reiniciamos el cambio que hicimos en nuestro arrey de piezas
        }
        else        //11.5 - si encontramos matches
        {
            ClearPieces(allMatches);        //11.5.1 - llamamos la funcion
        }

        //09.2.8 - se reinician los valores despues del movimiento
        startTile = null;
        endTile = null;
        swappingPieces = false;     //09.2.8.1 - le damos el valor de falso porque ya terminamos de hacer el intercambio de piezas

        yield return null;      //09.2.9 - para romper con esta coroutina
    }

    private void ClearPieces(List<Piece> piecesToClear)     //11.6 - funcion para eliminar las piezas y desencadenar las funciones para colapsar las piezas de arriba
    {
        piecesToClear.ForEach(piece =>      //11.6.1 - eliminar las piezas que recibimos
        {
            ClearPieceAt(piece.x, piece.y);     //11.6.1.1 - llama la funcion "ClearPieceAt"
        });

        List<int> columns = GetColumns(piecesToClear);      //11.6.2 - se obtienen las columnas en las que se eliminan las piezas y se llama la funcion "GetColumns"
        List<Piece> collapsedPieces =  collapseColumns(columns, 0.3f);       //11.6.3 - nos devuelve las piezas que se movieron y se llama a la funcion encargada de colapsar las piezas
    }

    private List<Piece> collapseColumns(List<int> columns, float timeToCollapse)    //11.8 - funcion para colapzar las piezas en las columnas
    {
        List<Piece> movingPieces = new List<Piece>();       //11.8.1 - creamos una lista con las piezas a mover

        for(int i = 0; i < columns.Count; i++)      //11.8.2 - se hace un for por cada una de las columnas
        {
            var column = columns[i];        //11.8.2.1 - se obtiene una referencia a la columna
            for (int y = 0; y < height; y++)        //11.8.2.2 - se va a iterar desde la fila inferior de la columna hasta el alto completo de la cuadricula
            {
                if (Pieces[column, y] == null)      //11.8.2.2.1 - se verificua si la pieza que se encuentra en esa posicion es nula
                {
                    for(int yplus = y +1; yplus < height; yplus++)      //11.8.2.2.1.1 - se le da la posicion a y + 1
                    {
                        if (Pieces[column, yplus] != null)      //11.8.2.2.1.1.1 - verificamos si ese espacion tiene una pieza que no sea nula
                        {
                            Pieces[column, yplus].Move(column, y);      //11.8.2.2.1.1.1.1 - movemosla pieza que podemos colapsar
                            Pieces[column, y] = Pieces[column, yplus];      //11.8.2.2.1.1.1.2 - actualizamos el sistema de coordenadas
                            if (!movingPieces.Contains(Pieces[column, y]))      //11.8.2.2.1.1.1.3 - verificamos si la pieza no esta en la lista de las piezas que se movieron
                            {
                                movingPieces.Add(Pieces[column, y]);        //11.8.2.2.1.1.1.3.1 - se agrega a las lista de las piezas que se movieron
                            }
                            Pieces[column, yplus] = null;       //11.8.2.2.1.1.1.4 - limpieamos la referencia de la pieza que movimos
                            break;      //11.8.2.2.1.1.1.5 - rompemos el for
                        }
                    }
                }
            }
        }
        return movingPieces;        //11.8.3 - retornamos la lista de piezas que se movieron para terminar la funcion
    }

    private List<int> GetColumns(List<Piece> piecesToClear)     //11.7 - se crea la funcion para agregar las columnas con los matches a eliminar
    {
        var result = new List<int>();       //11.7.1 - se crea la lista a devolver

        piecesToClear.ForEach(piece =>      //11.7.2 - se realiza el ForEach sobre piecesToClear
        {
            if (!result.Contains(piece.x))      //11.7.2.1 - se verifica si las columnas estan en la lista
            {
                result.Add(piece.x);        //11.7.2.1.1 - se agregan las columnas a lista
            }
        });

        return result;      //11.7.3 - se devuelve el resultado
    }

    public bool IsCloseTo(Tile start, Tile end)     //07.1 - esta funcion retorna un valor booleano y se agrega a 06.9.1
    {
        if (Math.Abs((start.x - end.x)) == 1 && start.y == end.y)        //07.1.1 - verificacion horizontal
        {       
            return true;
        }
        if (Math.Abs((start.y - end.y)) == 1 && start.x == end.x)   //07.1.2 - verificacion vertical
        {
            return true;
        }
        return false;
    }

    bool HasPreviousMatches(int posx, int posy)     //10.4 - funcion para impedir que se creen matches por defecto al iniciar la partida
    {
        var downMatches = GetMatchByDirection(posx, posy, new Vector2(0, -1), 2);   //10.4.1 - se buscan matches hacia abajo
        var leftMatches = GetMatchByDirection(posx, posy, new Vector2(-1, 0), 2);   //10.4.2 - se buscan marches hacia la izquierda

        if (downMatches == null) downMatches = new List<Piece>();        //10.4.3 - si tenemos listas nulas hacia abajo las inicializamos
        if (leftMatches == null) leftMatches = new List<Piece>();        //10.4.4 - si tenemos listas nulas hacia la izquierda las inicializamos

        return (downMatches.Count > 0 || leftMatches.Count > 0);     //10.4.5 - retornamos verdadero si encontramos algun match
    }

    public List<Piece> GetMatchByDirection(int xpos, int ypos, Vector2 direction, int minPieces = 3)    //08.1 - funcion que detecta los matches en una sola direccion
    {
        List<Piece> matches = new List<Piece>();     //08.1.1 - crea la lista de piezas de match que vamos a devolver
        Piece startPiece = Pieces[xpos, ypos];       //08.1.2 - encontrar la pieza inicial donde comenzara el match
        matches.Add(startPiece);                     //08.1.3 - agregamos la pieza inicial

        int nextX;      //08.1.4 - posicion en la que vamos a buscar una pieza
        int nextY;      //08.1.5 - posicion en la que vamos a buscar una pieza
        int maxVal = width > height ? width : height;        //08.1.6 - cual es el valor maximo al que podemos llegar antes de salinor de la cuadricula

        for (int i = 1; i < maxVal; i++)     ////08.1.7 - para iterar en la direccion que hemos escogido
        {
            nextX = xpos + ((int)direction.x * i);      //08.1.7.1 - se asignan los valores para mi proxima posicion en x que es donde voy a buscar si esa pieza es del mismo tipo que la anterior
            nextY = ypos + ((int)direction.y * i);      //08.1.7.2 - se asignan los valores para mi proxima posicion en y que es donde voy a buscar si esa pieza es del mismo tipo que la anterior

            if (nextX >= 0 && nextX < width && nextY >= 0 && nextY < height)      //08.1.7.3 - nos aseguramos que las posiciones anteriores sean mayores que 0 y menores que el ancho y el alto respectivamente
            {
                var nextPiece = Pieces[nextX, nextY];       //08.1.7.3.1 - obtenemos la referencia al a proxima pieza
                if (nextPiece != null && nextPiece.pieceType == startPiece.pieceType)      //08.1.7.3.2 - verificamos si esa proxima posicion tiene el mismo tipo de nuestra pieza inicial
                {
                    matches.Add(nextPiece);     //08.1.7.3.2.1 - agregamos la nueva pieza
                }
                else        //08.7.3.3 - si la tipo de la pieza no coincide se rompe el loop
                {
                    break;      //08.7.3.3.1 - se rompe
                }
            }
        }
        if (matches.Count >= minPieces)      //08.1.8 - verificamos si la cantidad de matches es mayor o igual a la cantidad minima de piezas
        {
            return matches;     //08.1.8.1 - devolvemos lo matches que encontramos
        }
        return null;        //08.1.9 - no devolvamos nada si no encontramos lo indicado
    }

    public List<Piece> GetMatchByPiece(int xpos, int ypos, int minPieces = 3)       //08.2 - esta funcion utilizara la funcion "GetMatchByDirection" para buscar matches en las 4 direcciones
    {
        //08.2.1 -  se utilizara la funcion "GetMatchByDirection" para cada una de las direcciones
        var upMatchs = GetMatchByDirection(xpos, ypos, new Vector2(0, 1), 2);      //08.2.1.1 - busca piezas hacia arriba
        var downMatchs = GetMatchByDirection(xpos, ypos, new Vector2(0, -1), 2);   //08.2.1.2 - busca piezas hacia abajo
        var rightMatchs = GetMatchByDirection(xpos, ypos, new Vector2(1, 0), 2);   //08.2.1.3 - busca piezas hacia la derecha
        var leftMatchs = GetMatchByDirection(xpos, ypos, new Vector2(-1, 0), 2);   //08.2.1.4 - busca piezas hacia la izquierda

        //08.2.3 - inicializamos las variables con listas vacias en caso de que retornen valores nulos
        if (upMatchs == null) upMatchs = new List<Piece>();
        if (downMatchs == null) downMatchs = new List<Piece>();
        if (rightMatchs == null) rightMatchs = new List<Piece>();
        if (leftMatchs == null) leftMatchs = new List<Piece>();

        //08.2.4 - vamos a unir las listas que recibimos por medio de una funcion llamada "union"
        var verticalMatches = upMatchs.Union(downMatchs).ToList();          //08.2.4.1 - unimos las listas verticales "upMatch + downMatch" como nos devuelve un arrey los convertimos a un lista "ToList"
        var horizontalMatches = leftMatchs.Union(rightMatchs).ToList();     //08.2.4.2 - unimos las listas horizontales "leftMatch + rightMatch" como nos devuelve un arrey los convertimos a un lista "ToList"

        var foundMatches = new List<Piece>();       //08.2.5 - creamos un lista que junte todos los matches que encontramos

        //08.2.6 - verificar si encontramos suficientes verticalMatches y suficientes horizontalMatches para contarlos dentro de los matches que vamos a devovler
        if (verticalMatches.Count >= minPieces)      //08.2.6.1 - se verifican los matches verticales
        {
            foundMatches = foundMatches.Union(verticalMatches).ToList();        //08.2.6.1.1 - unimos los matches en una lista
        }
        if (horizontalMatches.Count >= minPieces)      //08.2.6.2 - se verifican los matches verticales
        {
            foundMatches = foundMatches.Union(horizontalMatches).ToList();        //08.2.6.2.1 - unimos los matches en una lista
        }
        return foundMatches;        //08.2.7 - retornamos los foundMatches ya sea que encontremos o no
    }
}
