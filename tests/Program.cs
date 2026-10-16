// SPDX-FileCopyrightText: 2026 Demerzel Solutions Limited
// SPDX-License-Identifier: MIT

using System;
using System.IO;
using System.Runtime.InteropServices;
using Nethermind.Zkvm.Abstractions;

nint native = NativeLibrary.Load(Path.Combine(AppContext.BaseDirectory, "bitmap-test.dll"));
NativeLibrary.SetDllImportResolver(typeof(Accelerators).Assembly, (_, _, _) => native);

unsafe
{
    // Empty input needs neither a readable code buffer nor a writable bitmap.
    Accelerators.JumpDestBitmap(null, null, 0);

    ulong bitmap = ulong.MaxValue;
    ulong code = 0;
    Accelerators.JumpDestBitmap(&bitmap, (byte*)&code, 0);
    if (bitmap != ulong.MaxValue)
        throw new InvalidOperationException("Empty bytecode must leave the bitmap untouched.");

    delegate* unmanaged<int> callCount = (delegate* unmanaged<int>)NativeLibrary.GetExport(native, "bitmap_call_count");
    if (callCount() != 0)
        throw new InvalidOperationException("Empty bytecode must not invoke a native precompile.");

    Accelerators.JumpDestBitmap(&bitmap, (byte*)&code, 1);
    if (callCount() != 1)
        throw new InvalidOperationException("Nonempty bytecode must invoke the native precompile.");
}

Console.WriteLine("PASS: JumpDestBitmap skips empty bytecode (null and non-null buffers) and forwards nonempty bytecode.");
