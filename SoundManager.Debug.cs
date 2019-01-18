// =============================================================================
// MIT License
// 
// Copyright (c) 2018 Valeriya Pudova (hww.github.io)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// =============================================================================

using System.Text;
using Code.Display;
using UnityEngine;

namespace Plugins.VARP.Sounds
{
    public partial class SoundSystem
    {
        // TODO! MAKE IT WORK
        /*
        public static void Inspect(TextDisplay terminal)
        {
            var x = terminal.C;

            var curent = soundSourcesList.First;
            terminal.WriteFormat("inactive Sounds: {0}\n", SndSource.TotalSoundObjesInMemory);
            terminal.WriteFormat("active Sounds:   {0}\n", soundSourcesList.Count);
            while (curent != soundSourcesList.Root)
            {
                var node = curent.Data;
                var next = curent.Next;
                var snd = curent.Data;
                terminal.Write(snd.ToString());
                terminal.CursorLeft = x;
                terminal.CursorTop++;
                curent = next;
            }
        }
        */
        // TODO Make it controlable
        public static void PrintSoundManager()
        {
            var sb = new StringBuilder();
            sb.AppendLine("SoundManager complete report");
            Inspect(sb);
            Debug.Log(sb.ToString());
        }
        
        // TODO Make it controlable
        public static void Inspect(StringBuilder stringBuilder)
        {
            var curent = soundSourcesList.First;
            stringBuilder.AppendFormat("inactive sounds: {0}\n", TotalSoundObjesInMemory);
            stringBuilder.AppendFormat("active sounds:   {0}\n", soundSourcesList.Count);
            while (curent != null)
            {
                var next = curent.Next;
                var snd = curent.Value;
                stringBuilder.AppendLine(snd.ToString());
                curent = next;
            }
        }
    }
}