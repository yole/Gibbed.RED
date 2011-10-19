/* Copyright (c) 2011 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

namespace Gibbed.RED.FileFormats.Script
{
    public enum Opcode : byte
    {
        OP_Nop = 0,
        OP_Null = 1,
        OP_IntOne = 2,
        OP_IntZero = 3,
        OP_IntConst = 4,
        OP_ShortConst = 5,
        OP_FloatConst = 6,
        OP_StringConst = 7,
        OP_NameConst = 8,
        OP_ByteConst = 9,
        OP_BoolTrue = 10,
        OP_BoolFalse = 11,
        OP_Target = 12,
        OP_Assign = 13,
        OP_14 = 14,            // unused
        OP_LocalVar = 15,
        OP_ParamVar = 16,
        OP_ObjectVar = 17,
        OP_18 = 18,            // unused
        OP_Switch = 19,
        OP_SwitchLabel = 20,
        OP_SwitchDefault = 21,
        OP_Jump = 22,
        OP_JumpIfFalse = 23,
        OP_Skip = 24,
        OP_Conditional = 25,   // unused
        OP_Constructor = 26,
        OP_FinalFunc = 27,
        OP_VirtualFunc = 28,
        OP_EntryFunc = 29,
        
        OP_ParamEnd = 30,
        OP_Return = 31,
        OP_StructMember = 32,
        OP_Context = 33,
        OP_TestEqual = 34,
        OP_TestNotEqual = 35,  // unused
        OP_New = 36,
        OP_Delete = 37,        // unused
        OP_This = 38,
        OP_Parent = 39,
        OP_SavePoint = 40,
        OP_SaveValue = 41,
        OP_SavePointEnd = 42,

        OP_ArrayClear = 43,
        OP_ArraySize = 44,
        OP_ArrayResize = 45,
        OP_ArrayFindFirst = 46,
        OP_ArrayFindFirstFast = 47,
        OP_ArrayFindLast = 48,
        OP_ArrayFindLastFast = 49,
        OP_ArrayContains = 50,
        OP_ArrayContainsFast = 51,
        OP_ArrayPushBack = 52,
        OP_ArrayPopBack = 53,
        OP_ArrayInsert = 54,
        OP_ArrayRemove = 55,
        OP_ArrayRemoveFast = 56,
        OP_ArrayGrow = 57,
        OP_ArrayErase = 58,
        OP_ArrayLast = 59,
        OP_ArrayElement = 60,
        
        OP_BoolToByte = 61,
        OP_BoolToInt = 62,
        OP_BoolToFloat = 63,
        OP_BoolToString = 64,
        OP_ByteToBool = 65,
        OP_ByteToInt = 66,
        OP_ByteToFloat = 67,
        OP_ByteToString = 68,
        OP_IntToBool = 69,
        OP_IntToByte = 70,
        OP_IntToFloat = 71,
        OP_IntToString = 72,
        OP_FloatToBool = 73,
        OP_FloatToByte = 74,
        OP_FloatToInt = 75,
        OP_FloatToString = 76,
        OP_NameToBool = 77,
        OP_NameToString = 78,
        OP_StringToBool = 79,
        OP_StringToByte = 80,
        OP_StringToInt = 81,
        OP_StringToFloat = 82,
        OP_ObjectToBool = 83,
        OP_ObjectToString = 84,
        OP_EnumToString = 85,
        OP_EnumToInt = 86,

        OP_DynamicCast = 87,       // unused
        OP_GetGame = 88,
        OP_GetServer = 89,
        OP_GetPlayer = 90,
        OP_GetCamera = 91,
        OP_GetHud = 92,
        OP_GetSound = 93,
    }
}
