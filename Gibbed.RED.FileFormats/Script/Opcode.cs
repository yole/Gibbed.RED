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
        OP_LocalVar = 13,
        OP_ParamVar = 14,
        OP_DefaultVar = 15,
        OP_ObjectVar = 16,
        OP_Switch = 17,
        OP_SwitchLabel = 18,
        OP_SwitchDefault = 19,
        OP_Jump = 20,
        OP_JumpIfFalse = 21,
        OP_Skip = 22,
        OP_Conditional = 23,
        OP_Constructor = 24,
        OP_FinalFunc = 25,
        OP_VirtualFunc = 26,
        OP_ParamEnd = 27,
        OP_Return = 28,
        OP_StructMember = 29,
        OP_Context = 30,
        OP_Assign = 31,
        OP_TestEqual = 32,
        OP_TestNotEqual = 33,
        OP_New = 34,
        OP_Delete = 35,
        OP_This = 36,
        OP_Parent = 37,
        OP_SavePoint = 38,
        OP_SaveValue = 39,
        OP_SavePointEnd = 40,
        OP_Breakpoint = 41,
        OP_BoolToByte = 42,
        OP_BoolToInt = 43,
        OP_BoolToFloat = 44,
        OP_BoolToString = 45,
        OP_ByteToBool = 46,
        OP_ByteToInt = 47,
        OP_ByteToFloat = 48,
        OP_ByteToString = 49,
        OP_IntToBool = 50,
        OP_IntToByte = 51,
        OP_IntToFloat = 52,
        OP_IntToString = 53,
        OP_FloatToBool = 54,
        OP_FloatToByte = 55,
        OP_FloatToInt = 56,
        OP_FloatToString = 57,
        OP_NameToBool = 58,
        OP_NameToString = 59,
        OP_StringToBool = 60,
        OP_StringToByte = 61,
        OP_StringToInt = 62,
        OP_StringToFloat = 63,
        OP_ObjectToBool = 64,
        OP_ObjectToString = 65,
        OP_EnumToString = 66,
        OP_EnumToInt = 67,
        OP_DynamicCast = 68,
        OP_ArrayClear = 69,
        OP_ArraySize = 70,
        OP_ArrayResize = 71,
        OP_ArrayFindFirst = 72,
        OP_ArrayFindFirstFast = 73,
        OP_ArrayFindLast = 74,
        OP_ArrayFindLastFast = 75,
        OP_ArrayContains = 76,
        OP_ArrayContainsFast = 77,
        OP_ArrayPushBack = 78,
        OP_ArrayPopBack = 79,
        OP_ArrayInsert = 80,
        OP_ArrayRemove = 81,
        OP_ArrayRemoveFast = 82,
        OP_ArrayGrow = 83,
        OP_ArrayErase = 84,
        OP_ArrayLast = 85,
        OP_ArrayElement = 86,
        OP_EntryFunc = 87,
        OP_GetGame = 88,
        OP_GetServer = 89,
        OP_GetPlayer = 90,
        OP_GetCamera = 91,
        OP_GetHud = 92,
        OP_GetSound = 93,
    }
}
