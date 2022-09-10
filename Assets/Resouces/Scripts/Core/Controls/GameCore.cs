using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;
using PlayerInput;
using UnityEngine.InputSystem;


namespace GameCore
{
    public delegate void Vector2Handler(Vector2 vec);
    public delegate void FloatHandler(float value);
    public delegate void VoidHandler();
    public delegate void BoolHandler(bool value);
    public delegate void IntHandler(int value);
    public delegate void UIntHandler(uint value);

    public delegate uint UIntReturnVoidHandler();
}
