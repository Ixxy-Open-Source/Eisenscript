﻿using System;
using Eisenscript.Data_Structures;
using UnityEngine;

namespace Eisenscript
{
    public enum TokenType
    {
        // ReSharper disable IdentifierTypo
        White,
        Comment,
        Error,
        Mult,
        Colon,
        Greater,
        List,
        Image,
        Define,
        OpenBrace,
        CloseBrace,
        OpenBracket,
        CloseBracket,
        Comma,
        Number,
        Rgba,
        Variable,
        Set,
        Rule,
        MaxDepth,
        MaxObjects,
        MinSize,
        MaxSize,
        Seed,
        Initial,
        Background,
        Weight,
        X,
        Y,
        Z,
        Rx,
        Ry,
        Rz,
        S,
        M,
        Fx,
        Fy,
        Fz,
        Hue,
        Sat,
        Brightness,
        Alpha,
        Color,
        Blend,
        Random,
        RandomHue,
        RandomRgb,
        GreyScale,
        ColorPool,
        Box,
        Grid,
        Sphere,
        Line,
        Point,
        Triangle,
        Mesh,
        Cylinder,
        Tube,
        Translation,
        Rotation,
        Pivot,
        Scale,
        End
        // ReSharper restore IdentifierTypo
    }

    internal readonly struct Token
    {
        internal static bool IsObject(Token token)
        {
            return token.Type is
                TokenType.Box or
                TokenType.Grid or
                TokenType.Sphere or
                TokenType.Line or
                TokenType.Point or
                TokenType.Triangle or
                TokenType.Mesh or
                TokenType.Cylinder or
                TokenType.Tube;
        }

        private readonly double _value;
        private readonly string _name;
        private readonly Color _rgba;
        private readonly int _line;

        internal static readonly Trie<TokenType> Trie = new();

        internal int Line => _line;
        internal double Value
        {
            get
            {
                if (Type != TokenType.Number)
                {
                    throw new ParserException("Internal: Trying to get value from non-numeric token", _line);
                }
                return _value;
            }
        }

        internal Color Rgba
        {
            get
            {
                if (Type != TokenType.Rgba)
                {
                    throw new ParserException("Internal: Trying to get value from non-numeric token", _line);
                }
                return _rgba;
            }
        }

        internal string Name
        {
            get
            {
                if (Type != TokenType.Variable)
                {
                    throw new ParserException("Internal: Trying to get name from non-variable token", _line);
                }
                return _name;
            }
        }

        static Token()
        {
            // Tokens whose name isn't the same as the string
            Trie.Insert("*", TokenType.Mult);
            Trie.Insert(":", TokenType.Colon);
            Trie.Insert(">", TokenType.Greater);
            Trie.Insert("list:", TokenType.List);
            Trie.Insert("image:", TokenType.Image);
            Trie.Insert("#define", TokenType.Define);
            Trie.Insert("{", TokenType.OpenBrace);
            Trie.Insert("}", TokenType.CloseBrace);
            Trie.Insert("[", TokenType.OpenBracket);
            Trie.Insert("]", TokenType.CloseBracket);
            Trie.Insert(",", TokenType.Comma);

            // Abbreviations
            Trie.Insert("md", TokenType.MaxDepth);
            Trie.Insert("w", TokenType.Weight);
            Trie.Insert("b", TokenType.Brightness);
            Trie.Insert("a", TokenType.Alpha);

            for (var i = (int)TokenType.Variable + 1; i < (int)TokenType.End; i++)
            {
                var tt = (TokenType)i;
                Trie.Insert(Enum.GetName(typeof(TokenType), tt)!.ToLower(), tt);
            }
        }

        internal TokenType Type { get; }

        internal Token(TokenType type, int line)
        {
            Type = type;
            _line = line;
            _value = 0;
            _name = null;
            _rgba = default;
        }

        internal Token(double value, int line)
        {
            Type = TokenType.Number;
            _value = value;
            _line = line;
            _name = null;
            _rgba = default;
        }

        internal Token(Color rgba, int line)
        {
            Type = TokenType.Rgba;
            _rgba = rgba;
            _line = line;
            _value = 0;
            _name = null;
        }
        internal Token(string name, int line)
        {
            Type = TokenType.Variable;
            _name = name;
            _line = line;
            _value = 0;
            _rgba = default;
        }
    }
}
