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

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Gibbed.RED.FileFormats.Script
{
    internal static class OpcodeHandlerCache
    {
        private static Dictionary<Opcode, Type> Lookup = null;

        private static void BuildLookup()
        {
            Lookup = new Dictionary<Opcode, Type>();

            foreach (Type type in Assembly.GetAssembly(typeof(OpcodeHandlerCache)).GetTypes())
            {
                if (typeof(IInstruction).IsAssignableFrom(type) == true)
                {
                    foreach (OpcodeHandlerAttribute handler in type.GetCustomAttributes(typeof(OpcodeHandlerAttribute), false))
                    {
                        AddHandler(handler.Opcode, type);
                    }
                }
            }
        }

        private static void AddHandler(Opcode opcode, Type type)
        {
            if (Lookup.ContainsKey(opcode) == true)
            {
                throw new InvalidOperationException(string.Format("handler collision for {0}", opcode));
            }

            Lookup.Add(opcode, type);
        }

        public static IInstruction CreateInstruction(Opcode opcode)
        {
            if (Lookup == null)
            {
                BuildLookup();
            }

            if (Lookup.ContainsKey(opcode) == false)
            {
                throw new ArgumentException(string.Format("no handler for {0}", opcode));
            }

            return (IInstruction)Activator.CreateInstance(Lookup[opcode]);
        }
    }
}
