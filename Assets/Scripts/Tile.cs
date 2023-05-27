using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//03 - Sistema de coordenadas para instanciar las piezas
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
}
