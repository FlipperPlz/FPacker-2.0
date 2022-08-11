namespace FPackerLibrary.Compression; 

public class BohemiaLZO {
    public unsafe static uint decompress(byte* input, byte* output, uint expectedSize)
	{
		var ptr = output + expectedSize;
		var ptr2 = output;
		var ptr3 = input;
		if (*ptr3 <= 17) goto IL_3AA;
		var num = (uint)(*(ptr3++) - 17);
		if (num < 4U) goto IL_354;
		if ((long)(ptr - ptr2) < (long)((ulong)num))
		{
			throw new OverflowException("Outpur Overrun");
		}
		do
		{
			*(ptr2++) = *(ptr3++);
		}
		while ((num -= 1U) != 0U);
		IL_4E:
		num = (uint)(*(ptr3++));
		byte* ptr4;
		if (num < 16U)
		{
			ptr4 = ptr2 - (1U + M2_MAX_OFFSET);
			ptr4 -= num >> 2;
			ptr4 -= *(ptr3++) << 2;
			if (ptr4 < output || ptr4 >= ptr2)
			{
				throw new OverflowException("Lookbehind Overrun");
			}
			if ((long)(ptr - ptr2) < 3L)
			{
				throw new OverflowException("Output Overrun");
			}
			*(ptr2++) = *(ptr4++);
			*(ptr2++) = *(ptr4++);
			*(ptr2++) = *ptr4;
			goto IL_346;
		}
		IL_DC:
		if (num >= 64U)
		{
			ptr4 = ptr2 - 1;
			ptr4 -= (num >> 2 & 7U);
			ptr4 -= *(ptr3++) << 3;
			num = (num >> 5) - 1U;
			if (ptr4 < output || ptr4 >= ptr2)
			{
				throw new OverflowException("Lookbehind Overrun");
			}
			if ((long)(ptr - ptr2) < (long)((ulong)(num + 2U)))
			{
				throw new OverflowException("Output Overrun");
			}
		}
		else
		{
			if (num >= 32U)
			{
				num &= 31U;
				if (num == 0U)
				{
					while (*ptr3 == 0)
					{
						num += 255U;
						ptr3++;
					}
					num += (uint)(31 + *(ptr3++));
				}
				ptr4 = ptr2 - 1;
				ptr4 -= (*ptr3 >> 2) + ((int)ptr3[1] << 6);
				ptr3 += 2;
			}
			else if (num < 16U)
			{
				ptr4 = ptr2 - 1;
				ptr4 -= num >> 2;
				ptr4 -= *(ptr3++) << 2;
				if (ptr4 < output || ptr4 >= ptr2)
				{
					throw new OverflowException("Lookbehind Overrun");
				}
				if ((long)(ptr - ptr2) < 2L)
				{
					throw new OverflowException("Output Overrun");
				}
				*(ptr2++) = *(ptr4++);
				*(ptr2++) = *ptr4;
				goto IL_346;
			}
			else
			{
				ptr4 = ptr2;
				ptr4 -= (num & 8U) << 11;
				num &= 7U;
				if (num == 0U)
				{
					while (*ptr3 == 0)
					{
						num += 255U;
						ptr3++;
					}
					num += (uint)(7 + *(ptr3++));
				}
				ptr4 -= (*ptr3 >> 2) + ((int)ptr3[1] << 6);
				ptr3 += 2;
				if (ptr4 == ptr2)
				{
					long num2 = (long)(ptr2 - output);
					if (ptr4 != ptr)
					{
						throw new OverflowException("Output Underrun");
					}
					return (uint)((long)(ptr3 - input));
				}
				else
				{
					ptr4 -= 16384;
				}
			}
			if (ptr4 < output || ptr4 >= ptr2)
			{
				throw new OverflowException("Lookbehind Overrun");
			}
			if ((long)(ptr - ptr2) < (long)((ulong)(num + 2U)))
			{
				throw new OverflowException("Output Overrun");
			}
			if (num >= 6U && (long)(ptr2 - ptr4) >= 4L)
			{
				*(int*)ptr2 = *(int*)ptr4;
				ptr2 += 4;
				ptr4 += 4;
				num -= 2U;
				do
				{
					*(int*)ptr2 = *(int*)ptr4;
					ptr2 += 4;
					ptr4 += 4;
					num -= 4U;
				}
				while (num >= 4U);
				if (num != 0U)
				{
					do
					{
						*(ptr2++) = *(ptr4++);
					}
					while ((num -= 1U) != 0U);
					goto IL_346;
				}
				goto IL_346;
			}
		}
		*(ptr2++) = *(ptr4++);
		*(ptr2++) = *(ptr4++);
		do
		{
			*(ptr2++) = *(ptr4++);
		}
		while ((num -= 1U) != 0U);
		IL_346:
		num = (uint)(ptr3[-2] & 3);
		if (num == 0U)
		{
			goto IL_3AA;
		}
		IL_354:
		if ((long)(ptr - ptr2) < (long)((ulong)num))
		{
			throw new OverflowException("Output Overrun");
		}
		*(ptr2++) = *(ptr3++);
		if (num > 1U)
		{
			*(ptr2++) = *(ptr3++);
			if (num > 2U)
			{
				*(ptr2++) = *(ptr3++);
			}
		}
		num = (uint)(*(ptr3++));
		goto IL_DC;
		IL_3AA:
		num = (uint)(*(ptr3++));
		if (num >= 16U)
		{
			goto IL_DC;
		}
		if (num == 0U)
		{
			while (*ptr3 == 0)
			{
				num += 255U;
				ptr3++;
			}
			num += (uint)(15 + *(ptr3++));
		}
		if ((long)(ptr - ptr2) < (long)((ulong)(num + 3U)))
		{
			throw new OverflowException("Output Overrun");
		}
		*(int*)ptr2 = *(int*)ptr3;
		ptr2 += 4;
		ptr3 += 4;
		if ((num -= 1U) == 0U)
		{
			goto IL_4E;
		}
		if (num < 4U)
		{
			do
			{
				*(ptr2++) = *(ptr3++);
			}
			while ((num -= 1U) != 0U);
			goto IL_4E;
		}
		do
		{
			*(int*)ptr2 = *(int*)ptr3;
			ptr2 += 4;
			ptr3 += 4;
			num -= 4U;
		}
		while (num >= 4U);
		if (num != 0U)
		{
			do
			{
				*(ptr2++) = *(ptr3++);
			}
			while ((num -= 1U) != 0U);
			goto IL_4E;
		}
		goto IL_4E;
	}
	// Token: 0x060001C6 RID: 454 RVA: 0x00008C8C File Offset: 0x00006E8C
	private static byte ip( System.IO.Stream i )
	{
		byte result = (byte)i.ReadByte();
		long position = i.Position;
		i.Position = position - 1L;
		return result;
	}
	// Token: 0x060001C7 RID: 455 RVA: 0x00008CB8 File Offset: 0x00006EB8
	private static byte ip( System.IO.Stream i, short offset)
	{
		i.Position += (long)offset;
		byte result = (byte)i.ReadByte();
		i.Position -= (long)(offset + 1);
		return result;
	}
	// Token: 0x060001C8 RID: 456 RVA: 0x00008CF0 File Offset: 0x00006EF0
	private static byte next( System.IO.Stream i )
	{
		return (byte)i.ReadByte();
	}
	// Token: 0x060001C9 RID: 457 RVA: 0x00008CFC File Offset: 0x00006EFC
	public unsafe static uint decompress( System.IO.Stream i, byte* output, uint expectedSize)
	{
		long position = i.Position;
		byte* ptr = output + expectedSize;
		byte* ptr2 = output;
		if (ip(i) <= 17)
		{
			goto IL_404;
		}
		uint num = (uint)(next(i) - 17);
		if (num < 4U)
		{
			goto IL_3AE;
		}
		if ((long)(ptr - ptr2) < (long)((ulong)num))
		{
			throw new OverflowException("Outpur Overrun");
		}
		do
		{
			*(ptr2++) = next(i);
		}
		while ((num -= 1U) != 0U);
		IL_57:
		num = (uint)next(i);
		byte* ptr3;
		if (num < 16U)
		{
			ptr3 = ptr2 - (1U + M2_MAX_OFFSET);
			ptr3 -= num >> 2;
			ptr3 -= next(i) << 2;
			if (ptr3 < output || ptr3 >= ptr2)
			{
				throw new OverflowException("Lookbehind Overrun");
			}
			if ((long)(ptr - ptr2) < 3L)
			{
				throw new OverflowException("Output Overrun");
			}
			*(ptr2++) = *(ptr3++);
			*(ptr2++) = *(ptr3++);
			*(ptr2++) = *ptr3;
			goto IL_39D;
		}
		IL_E5:
		if (num >= 64U)
		{
			ptr3 = ptr2 - 1;
			ptr3 -= (num >> 2 & 7U);
			ptr3 -= next(i) << 3;
			num = (num >> 5) - 1U;
			if (ptr3 < output || ptr3 >= ptr2)
			{
				throw new OverflowException("Lookbehind Overrun");
			}
			if ((long)(ptr - ptr2) < (long)((ulong)(num + 2U)))
			{
				throw new OverflowException("Output Overrun");
			}
		}
		else
		{
			if (num >= 32U)
			{
				num &= 31U;
				if (num == 0U)
				{
					while (ip(i) == 0)
					{
						num += 255U;
						long position2 = i.Position;
						i.Position = position2 + 1L;
					}
					num += (uint)(31 + next(i));
				}
				ptr3 = ptr2 - 1;
				ptr3 -= (ip(i, 0) >> 2) + ((int)ip(i, 1) << 6);
				i.Position += 2L;
			}
			else if (num < 16U)
			{
				ptr3 = ptr2 - 1;
				ptr3 -= num >> 2;
				ptr3 -= next(i) << 2;
				if (ptr3 < output || ptr3 >= ptr2)
				{
					throw new OverflowException("Lookbehind Overrun");
				}
				if ((long)(ptr - ptr2) < 2L)
				{
					throw new OverflowException("Output Overrun");
				}
				*(ptr2++) = *(ptr3++);
				*(ptr2++) = *ptr3;
				goto IL_39D;
			}
			else
			{
				ptr3 = ptr2;
				ptr3 -= (num & 8U) << 11;
				num &= 7U;
				if (num == 0U)
				{
					while (ip(i) == 0)
					{
						num += 255U;
						long position2 = i.Position;
						i.Position = position2 + 1L;
					}
					num += (uint)(7 + next(i));
				}
				ptr3 -= (ip(i, 0) >> 2) + ((int)ip(i, 1) << 6);
				i.Position += 2L;
				if (ptr3 == ptr2)
				{
					long num2 = (long)(ptr2 - output);
					if (ptr3 != ptr)
					{
						throw new OverflowException("Output Underrun");
					}
					return (uint)(i.Position - position);
				}
				else
				{
					ptr3 -= 16384;
				}
			}
			if (ptr3 < output || ptr3 >= ptr2)
			{
				throw new OverflowException("Lookbehind Overrun");
			}
			if ((long)(ptr - ptr2) < (long)((ulong)(num + 2U)))
			{
				throw new OverflowException("Output Overrun");
			}
			if (num >= 6U && (long)(ptr2 - ptr3) >= 4L)
			{
				*(int*)ptr2 = *(int*)ptr3;
				ptr2 += 4;
				ptr3 += 4;
				num -= 2U;
				do
				{
					*(int*)ptr2 = *(int*)ptr3;
					ptr2 += 4;
					ptr3 += 4;
					num -= 4U;
				}
				while (num >= 4U);
				if (num != 0U)
				{
					do
					{
						*(ptr2++) = *(ptr3++);
					}
					while ((num -= 1U) != 0U);
					goto IL_39D;
				}
				goto IL_39D;
			}
		}
		*(ptr2++) = *(ptr3++);
		*(ptr2++) = *(ptr3++);
		do
		{
			*(ptr2++) = *(ptr3++);
		}
		while ((num -= 1U) != 0U);
		IL_39D:
		num = (uint)(ip(i, -2) & 3);
		if (num == 0U)
		{
			goto IL_404;
		}
		IL_3AE:
		if ((long)(ptr - ptr2) < (long)((ulong)num))
		{
			throw new OverflowException("Output Overrun");
		}
		*(ptr2++) = next(i);
		if (num > 1U)
		{
			*(ptr2++) = next(i);
			if (num > 2U)
			{
				*(ptr2++) = next(i);
			}
		}
		num = (uint)next(i);
		goto IL_E5;
		IL_404:
		num = (uint)next(i);
		if (num >= 16U)
		{
			goto IL_E5;
		}
		if (num == 0U)
		{
			while (ip(i) == 0)
			{
				num += 255U;
				long position2 = i.Position;
				i.Position = position2 + 1L;
			}
			num += (uint)(15 + next(i));
		}
		if ((long)(ptr - ptr2) < (long)((ulong)(num + 3U)))
		{
			throw new OverflowException("Output Overrun");
		}
		*(ptr2++) = next(i);
		*(ptr2++) = next(i);
		*(ptr2++) = next(i);
		*(ptr2++) = next(i);
		if ((num -= 1U) == 0U)
		{
			goto IL_57;
		}
		if (num < 4U)
		{
			do
			{
				*(ptr2++) = next(i);
			}
			while ((num -= 1U) != 0U);
			goto IL_57;
		}
		do
		{
			*(ptr2++) = next(i);
			*(ptr2++) = next(i);
			*(ptr2++) = next(i);
			*(ptr2++) = next(i);
			num -= 4U;
		}
		while (num >= 4U);
		if (num != 0U)
		{
			do
			{
				*(ptr2++) = next(i);
			}
			while ((num -= 1U) != 0U);
			goto IL_57;
		}
		goto IL_57;
	}
	// Token: 0x060001CA RID: 458 RVA: 0x00009224 File Offset: 0x00007424
	public unsafe static uint readLZO( System.IO.Stream input, out byte[] dst, uint expectedSize)
	{
		dst = new byte[expectedSize];
		fixed (byte* ptr = &dst[0])
		{
			byte* output = ptr;
			return decompress(input, output, expectedSize);
		}
	}
	// Token: 0x060001CB RID: 459 RVA: 0x00009254 File Offset: 0x00007454
	public unsafe static byte[] readLZO( System.IO.Stream input, uint expectedSize)
	{
		byte[] array = new byte[expectedSize];
		fixed (byte* ptr = &array[0])
		{
			byte* output = ptr;
			decompress(input, output, expectedSize);
		}
		return array;
	}
	// Token: 0x040001BF RID: 447
	private static readonly uint M2_MAX_OFFSET = 2048U;
}