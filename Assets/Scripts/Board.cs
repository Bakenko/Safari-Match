using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

//01 - Creando la cuadricula
//02 - Ajustando la camara
//03 - Sistema de coordenadas para instanciar las piezas

public class Board : MonoBehaviour
{
    public int width;               //01.1 - ancho de la cuadricula
    public int height;              //01.2 - alto de la cuadricula
    public GameObject tileObject;   //01.3 - objeto por defecto para cada espacio de la caudricula (empty)

    public float cameraSizeOffset;  //02.2 - agregar un numero al tamaño ortografico final de la camara (se agrega a 02.1.6)
    public float cameraVerticalOffset;  //02.3 - permite agregar un valor a la posicion en vertical de la camara (se agrega a 02.1.3)

        
    // Start is called before the first frame update
    void Start()
    {
        SetupBoard();               //se llama la funcion encargada de crear la cuadricula
        PositionCamera();           //se llama la funcion encargada de ajustar la camara
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
                var o = Instantiate(tileObject, new Vector3(x, y, -5), Quaternion.identity);    //01.4.1.1.2 - cremos nuestros objetos
                o.transform.parent = transform;     //01.4.1.1.3 - hacer que el padre del objeto sea la board
                o.GetComponent<Tile>()?.Setup(x, y, this);  //03.5 - para tener acceso al componente de tipo tile y llamar la funcion setup
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
