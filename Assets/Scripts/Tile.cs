using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//03 - Sistema de coordenadas para instanciar las piezas
//06 - Intercambiando las piezas de lugar

public class Tile : MonoBehaviour
{
    public int x;       //03.1 - propiedad para la coordenada x
    public int y;       //03.2 - propiedad para la coordenada y
    public Board board;     //03.3 - referencia a la cuadricula

    public void Setup(int x_, int y_, Board board_)     //03.4 - para llevar cuenta de que espacion representa cada uno de los tile que tenemos
    {
        x = x_;
        y = y_;
        board = board_;
    }

    //06.12 - agregar el input del mouse para poder clickear en los espacios y llamar las funciones del board
    public void OnMouseDown()   //06.12.1 - cuando hagamos click sobre el objeto
    {
        board.TileDown(this);   //06.12.1.1 - se llama la funcion
    }

    public void OnMouseEnter()  //06.12.2 - cuando arrastro el mouse
    {
        board.TileOver(this);   //06.12.2.1 - se llama la funcion
    }

    public void OnMouseUp()     //06.12.3 - cuando levantemos el mouse sobre este elemento
    {
        board.TileUp(this);     //06.12.3.1 - se llama la funcion
    }
}
