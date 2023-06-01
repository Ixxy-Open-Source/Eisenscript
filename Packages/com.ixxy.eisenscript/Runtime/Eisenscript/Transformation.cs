using System;
using Eisenscript;
using UnityEngine;
using Color = UnityEngine.Color;

public class Transformation
{
    #region Values
    public Matrix4x4 Mtx { get; }

    // For color alterations
#pragma warning disable CS0414
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    private readonly float _deltaH;
    private readonly float _scaleS;
    private readonly float _scaleB;
    private readonly float _scaleAlpha;
    internal bool IsAbsoluteColor { get; set; }
    internal Color AbsoluteColor { get; set; }
    internal bool IsRandomColor { get; set; }

    // For color blends
    private float _hBlend;
    private float _sBlend;
    private float _vBlend;
    private float _aBlend;
    internal float Strength { get; set; }
    private bool _hsbRequired;
    private bool _colorAlteration;
    private bool _colorValidated;
    internal Color BlendColor
    {
        set
        {
            Color.RGBToHSV(value, out float _hBlend, out float _sBlend, out float _vBlend);
            _aBlend = value.a;
        }
    }
    // ReSharper restore UnusedAutoPropertyAccessor.Global
#pragma warning restore CS0414
    #endregion

    #region Constructor
    internal Transformation(Matrix4x4 mtx,
        float deltaH = 0.0f,
        float scaleS = 1.0f,
        float scaleB = 1.0f,
        float scaleAlpha = 1.0f)
    {
        scaleS = Math.Max(scaleS, 0);
        scaleB = Math.Max(scaleB, 0);
        Mtx = mtx;
        _deltaH = deltaH;
        _scaleS = scaleS;
        _scaleB = scaleB;
        _scaleAlpha = scaleAlpha;
    }
    #endregion

    #region Transformation
    public (Matrix4x4, Color, bool colorChanged) DoTransform(Matrix4x4 matrix, Color rgba, Rules rules)
    {
        if (!_colorValidated)
        {
            _colorValidated = true;
            // ReSharper disable CompareOfFloatsByEqualityOperator
            _hsbRequired = _deltaH != 0 || _scaleS != 1 || _scaleB != 1 || Strength != 0;
            _colorAlteration = _hsbRequired || _scaleAlpha != 1 || Strength != 0 || IsAbsoluteColor || IsRandomColor;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }
        var retMatrix = Mtx * matrix;
        var retRgba = rgba;

        if (!_colorAlteration)
        {
            return (retMatrix, retRgba, false);
        }
        if (IsAbsoluteColor)
        {
            return (retMatrix, AbsoluteColor, true);
        }

        if (IsRandomColor)
        {
            return (retMatrix, rules.Pool.ChooseColor(rules.RndColors), true);
        }

        if (_hsbRequired)
        {
            // TODO: Am I getting HSV when I should be getting HSB?  Not sure.
            Color.RGBToHSV(rgba, out float h, out float s, out float b);
            if (_deltaH != 0)
            {
                h = (float)((h + _deltaH) % 360f);
            }

            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (_scaleS != 1)
            {
                s = Mathf.Max(1, s * _scaleS);
            }

            if (_scaleB != 1)
            {
                b = Mathf.Max(1, b * _scaleB);
            }
            // ReSharper restore CompareOfFloatsByEqualityOperator

            if (Strength != 0)
            {
                retRgba = LerpHSV.Lerp(h, s, b, _hBlend, _sBlend, _vBlend, Strength);
            }

            retRgba.a = rgba.a;
        }

        if (_scaleAlpha != 0)
        {
            retRgba.a = (byte)Math.Min(255.99, _scaleAlpha * rgba.a + 0.5);
        }

        if (Strength != 0)
        {
            retRgba.a = (byte)(rgba.a * (1 - Strength) + _aBlend * Strength);
        }
        return (retMatrix, retRgba, true);
    }
    #endregion

    #region Parsing
    internal static Transformation ParseTransform(Scan scan)
    {
        scan.Consume(TokenType.OpenBrace);

        var matrix = Matrix4x4.identity;
        var deltaH = 0.0f;
        var scaleB = 1.0f;
        var scaleS = 1.0f;
        var scaleAlpha = 1.0f;
        var absoluteColor = new Color();
        var isAbsoluteColor = false;
        var blendColor = new Color();
        var strength = 0.0f;
        var isRandomColor = false;

        while (scan.Peek().Type != TokenType.CloseBrace)
        {
            var type = scan.Peek().Type;
            scan.Advance();
            switch (type)
            {
                case TokenType.X:
                    matrix = Matrix4x4.Translate(new Vector3((float)scan.NextDouble(), 0, 0)) * matrix;
                    break;

                case TokenType.Y:
                    matrix = Matrix4x4.Translate(new Vector3(0, (float)scan.NextDouble(), 0)) * matrix;
                    break;

                case TokenType.Z:
                    matrix = Matrix4x4.Translate(new Vector3(0, 0, (float)scan.NextDouble())) * matrix;
                    break;

                case TokenType.Rx:
                    matrix = Matrix4x4.Rotate(Quaternion.Euler((Mathf.PI / 180) * (float)scan.NextDouble(), 0, 0)) * matrix;
                    break;

                case TokenType.Ry:
                    matrix = Matrix4x4.Rotate(Quaternion.Euler(0, (Mathf.PI / 180) * (float)scan.NextDouble(), 0)) * matrix;
                    break;

                case TokenType.Rz:
                    matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, (Mathf.PI / 180) * (float)scan.NextDouble())) * matrix;
                    break;

                case TokenType.S:
                    var scaleX = (float)scan.NextDouble();
                    if (scan.Peek().Type != TokenType.Number)
                    {
                        matrix = Matrix4x4.Scale(Vector3.one * scaleX) * matrix;
                        break;
                    }

                    var scaleY = (float)scan.NextDouble();
                    var scaleZ = (float)scan.NextDouble();
                    matrix = Matrix4x4.Scale(new Vector3(scaleX, scaleY, scaleZ)) * matrix;
                    break;

                case TokenType.M:
                    var m11 = (float)scan.NextDouble();
                    var m12 = (float)scan.NextDouble();
                    var m13 = (float)scan.NextDouble();
                    var m21 = (float)scan.NextDouble();
                    var m22 = (float)scan.NextDouble();
                    var m23 = (float)scan.NextDouble();
                    var m31 = (float)scan.NextDouble();
                    var m32 = (float)scan.NextDouble();
                    var m33 = (float)scan.NextDouble();
                    matrix = new Matrix4x4(
                        new Vector4(m11, m12, m13, 0),
                        new Vector4(m21, m22, m23, 0),
                        new Vector4(m31, m32, m33, 0),
                        new Vector4(0, 0, 0, 1)) * matrix;
                    break;

                case TokenType.Fx:
                    matrix = Matrix4x4.Scale(new Vector3(-1, 0, 0)) * matrix;
                    break;

                case TokenType.Fy:
                    matrix = Matrix4x4.Scale(new Vector3(0, -1, 0)) * matrix;
                    break;

                case TokenType.Fz:
                    matrix = Matrix4x4.Scale(new Vector3(0, 0, -1)) * matrix;
                    break;

                case TokenType.Hue:
                    deltaH = (float)scan.NextDouble();
                    break;

                case TokenType.Sat:
                    scaleS = (float)scan.NextDouble();
                    break;

                case TokenType.Brightness:
                    scaleB = (float)scan.NextDouble();
                    break;

                case TokenType.Alpha:
                    scaleAlpha = (float)scan.NextDouble();
                    break;

                case TokenType.Color:
                    if (scan.Peek().Type == TokenType.Random)
                    {
                        scan.Advance();
                        isRandomColor = true;
                        break;
                    }
                    absoluteColor = scan.NextRgba();
                    isAbsoluteColor = true;
                    break;

                case TokenType.Blend:
                    blendColor = scan.NextRgba();
                    strength = (float)scan.NextDouble();
                    break;
            }
        }

        scan.Consume(TokenType.CloseBrace);
        return new Transformation(matrix, deltaH, scaleS, scaleB, scaleAlpha)
        {
            AbsoluteColor = absoluteColor,
            IsAbsoluteColor = isAbsoluteColor,
            BlendColor = blendColor,
            Strength = strength,
            IsRandomColor = isRandomColor,
        };
    }

    #endregion
}