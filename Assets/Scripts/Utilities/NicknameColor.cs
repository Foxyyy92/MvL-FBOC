using System;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;

namespace NSMB.Utilities {
    public unsafe struct NicknameColor {
        private const int ValueCount = 3 * 4;
        public fixed float values[ValueCount];
        public bool Constant => this[1] == Vector3.zero;
        public Vector3 this[int i] {
            get => new Vector3(values[i*3], values[i*3+1], values[i*3+2]);
            set {
                values[i/3] = value.x;
                values[i/3+1] = value.y;
                values[i/3+2] = value.z;
            }
        }

        public Color Sample() {
            float time = (float)((Time.timeAsDouble * 0.1d) % 1d);

        // Gather colors
        Vector3[] points = new Vector3[] { this[0], this[1], this[2], this[3] };

        // Filter out redundant zero slots
        List<Vector3> valid = new();
        foreach (var p in points) {
            if (p != Vector3.zero || valid.Count == 0) valid.Add(p);
        }

        return Utils.SampleLoopingLinearGradient(time, valid.ToArray());
    }

        public override string ToString() {
            return $"NicknameColor[a={this[0]} b={this[1]} c={this[2]} d={this[3]}]";
        }

        public static NicknameColor White {
            get {
                NicknameColor ret = new NicknameColor();
                ret[0] = Vector3.one;
                return ret;
            }
        }
        public static NicknameColor Parse(ReadOnlySpan<char> colorStr) {
            ReadOnlySpan<char> original = colorStr;
            NicknameColor ret = White;

            if (colorStr.IsEmpty) {
                return ret;
            }

            try {
                if (colorStr[0] == '#') {
                    // Solid color
                    ret[0] = new Vector3(
                        byte.Parse(colorStr[1..3], NumberStyles.HexNumber) / 255f,
                        byte.Parse(colorStr[3..5], NumberStyles.HexNumber) / 255f,
                        byte.Parse(colorStr[5..7], NumberStyles.HexNumber) / 255f);
                } else {
                    // Gradient
                    int i = 0;
                    while (i < ValueCount && !colorStr.IsEmpty) {
                        int separator = colorStr.IndexOf(',');
                        if (separator != -1) {
                            ret.values[i] = float.Parse(colorStr[0..separator], provider: CultureInfo.InvariantCulture);
                            colorStr = colorStr[(separator + 1)..];
                            i++;
                        } else {
                            ret.values[i] = float.Parse(colorStr, provider: CultureInfo.InvariantCulture);
                            break;
                        }
                    }
                }
            } catch (Exception e) {
                Debug.LogWarning($"Failed to parse Nickname Color '{original.ToString()}'");
                Debug.LogWarning(e);
            }

            return ret;
        }
    }
}
