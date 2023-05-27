using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;  //05.1 - libreria de dotween

//04 - Instanciando las piezas en la cuadrícula
//05 - Moviendo las piezas
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

    public void Move(int desX, int desY)        //05.2 - movimiento de la pieza
    {
        transform.DOMove(new Vector3(desX, desY, -5f), 0.25f).SetEase(Ease.InOutCubic).onComplete = () =>   //05.2.1 - SetEase suaviza el movimiento y onComplete actualiza las posicion de la piezas despues del movimiento
        {
            x = desX;
            y = desY;
        };
    }

    [ContextMenu("Test Move")]      //05.3 - Decorador
    public void MoveTest()      //05.4 - funcion de test para poder verlo dentro de Unity
    {
        Move(0, 0);
    }
}
