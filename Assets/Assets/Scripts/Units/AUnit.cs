using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class AUnit : MonoBehaviour
{
    /**
     * Abstraktni metoda pro pridani jednotky do party.
     **/
    public abstract void AddToGroup();

    /**
     * Abstraktni metoda pro odstraneni jednotky z party.
     **/
    public abstract void RemoveFromGroup();

    /**
     * Abstraktni metoda pro ziskani smeru pohybu.
     **/
    public abstract void GetMovementDirection();


}
