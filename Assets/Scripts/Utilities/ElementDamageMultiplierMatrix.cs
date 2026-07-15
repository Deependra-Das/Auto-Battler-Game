using UnityEngine;

public static class ElementDamageMultiplierMatrix
{

    // rows = source
    // columns = target

    private static readonly float[,] multipliers =
    {
        //              Physical  Fire  Thunder  Nature
        /* Physical */ { 2.0f,    1.0f,  1.0f,   1.0f },
        /* Fire     */ { 1.0f,    1.0f,  0.5f,   2.0f },
        /* Thunder  */ { 1.0f,    2.0f,  1.0f,   0.5f },
        /* Nature   */ { 1.0f,    0.5f,  2.0f,   1.0f }
    };

    public static float GetMultiplier(UnitElementEnum damageSourceElement, UnitElementEnum targetElement)
    {
        return multipliers[(int)damageSourceElement, (int)targetElement];
    }    
}
