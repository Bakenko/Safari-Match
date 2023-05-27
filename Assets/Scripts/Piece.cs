using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//04 - Instanciando las piezas en la cuadrícula
public class Piece : MonoBehaviour
{
    public int x;       //04.1 - propiedad para la coordenada x
    public int y;       //04.2 - propiedad para la coordenada y
    public Board board;     //04.3 - referencia a la cuadricula

    public enum type    //04.4 - representa el tipo de las imagenes en un enum (similar a una lista de la que se puede esoger una opcion)
    {
        elephant,
        giraffe,
        hippo,
        monkey,
        panda,
        parrot,
        penguin,
        pig,
        rabbit,
        snake
    };

    public type pieceType;  //04.5 - guarda el tipo de la pieza actual

    public void Setup(int x_, int y_, Board board_)     //04.6 - para llevar cuenta de que espacion representa cada uno de los tile que tenemos
    {
        x = x_;
        y = y_;
        board = board_;
    }
}
