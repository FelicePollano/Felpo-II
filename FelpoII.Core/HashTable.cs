﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace FelpoII.Core
{
    public static class HashTable
    {
        static IntPtr hashTable = IntPtr.Zero;
        static int hashEntries;
        static int mask;
        public static int Initialize(int keyPortionToHash)
        {
            unsafe
            {
                mask = hashEntries = keyPortionToHash;
                if (hashTable != IntPtr.Zero)
                    Marshal.FreeHGlobal(hashTable);
                int size = Marshal.SizeOf(typeof(HashTableEntry)) * hashEntries + Marshal.SizeOf(typeof(HashTableEntry));
                hashTable = Marshal.AllocHGlobal(size);
                return size;
            }
        }
        public static void Drop()
        {
            if (hashTable != IntPtr.Zero)
                Marshal.FreeHGlobal(hashTable);
            hashTable = IntPtr.Zero;
        }

        public static void StoreMovesCount(ulong zkey, ulong moves, int depth)
        {
            if (0 == hashEntries)
                return;
            int hash = GetHash(zkey);
            unsafe
            {
                HashTableEntry* h = (HashTableEntry*)hashTable;
                h[hash].Payload = moves;
                h[hash].Type = HashTableEntry.MoveCountType;
                h[hash].Key  = zkey ;
                h[hash].Depth = (byte)depth;
            }
        }

        private static unsafe bool KeyConfirm(HashTableEntry* h,int hash, ulong zkey)
        {
            return h[hash].Key == (zkey & ~HashTableEntry.KeyMask);
        }
        private static int GetHash(ulong zkey)
        {
            return (int)(zkey & (ulong)mask);
        }
        public static bool ProbeMovesCount(ulong zkey, out ulong moves, int depth)
        {
            if (0 == hashEntries)
            {
                moves = 0;
                return false;
            }
            int hash = GetHash(zkey);
            unsafe
            {
                HashTableEntry* h = (HashTableEntry*)hashTable;
                if (KeyConfirm(h,hash,zkey) && 0 != (h[hash].Type & HashTableEntry.MoveCountType) && h[hash].Depth == depth)
                {
                    moves = h[hash].Payload;
                    return true;
                }
                else
                {
                    moves = 0;
                    return false;
                }
            }
        }


        public static bool Probe(ulong zkey, int depth, int alpha, int beta, ref int ttMove, ref int score)
        {

            if (0 == hashEntries)
            {
                return false;
            }
            int hash = GetHash(zkey);
            unsafe
            {
                HashTableEntry* h = (HashTableEntry*)hashTable;
                if (KeyConfirm(h, hash, zkey) )
                {
                    if (h[hash].Depth >= depth)
                    {
                        ttMove = (int)(h[hash].Payload & 0xFFFFFFFFUL);
                        int val = (int)(h[hash].Payload>>32);
                        if (h[hash].Type == HashTableEntry.ExactType)
                        {
                            score = val;
                            return true;
                        }

                        if (h[hash].Type == HashTableEntry.AlphaType && (val <= alpha))
                        {
                            score = alpha;
                            return true;
                        }
                        if (h[hash].Type == HashTableEntry.BetaType && (val >= beta))
                        {
                            score = beta;
                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    
                    return false;
                }
            }
        }

        public static void Save(ulong zkey, int depth, int score, byte type, int move)
        {
            if (0 == hashEntries)
                return;
            int hash = GetHash(zkey);
            unsafe
            {
                HashTableEntry* h = (HashTableEntry*)hashTable;
                h[hash].Payload = (ulong)(((ulong)score << 32) | (uint)move);
                h[hash].Type = type;
                h[hash].Key = zkey;
                h[hash].Depth = (byte)depth;
            }
        }
    }
}
