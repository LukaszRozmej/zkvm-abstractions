// SPDX-FileCopyrightText: 2026 Demerzel Solutions Limited
// SPDX-License-Identifier: MIT

#include <stddef.h>
#include <stdint.h>

#ifdef _WIN32
#define EXPORT __declspec(dllexport)
#else
#define EXPORT __attribute__((visibility("default")))
#endif

static int calls;

EXPORT void zkvm_jump_dest_bitmap(uint64_t *bitmap, const uint8_t *code, size_t length)
{
    (void)bitmap;
    (void)code;
    (void)length;
    calls++;
}

EXPORT int bitmap_call_count(void)
{
    return calls;
}
