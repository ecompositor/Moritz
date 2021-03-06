﻿using System;
using System.Xml;
using System.Text;
using System.Diagnostics;

using Moritz.Globals;

namespace Moritz.Xml
{
    public class SvgWriter : XmlWriterWrapper
    {
        public SvgWriter(string file, XmlWriterSettings settings)
            : base(XmlWriter.Create(file, settings))
        {
        }

        #region WriteSVG primitives
        /// <summary>
        /// Starts an SVG "g" element. End the group with WriteEndGroup().
        /// </summary>
        /// <param name="type">Can be null or empty or a class attribute</param>
        /// <param name="id">Can be null or empty or an id attribute</param>
        public void SvgStartGroup(string type, string id)
        {
            _w.WriteStartElement("g");

            if(!String.IsNullOrEmpty(type))
                _w.WriteAttributeString("class", type);

            if(!String.IsNullOrEmpty(id))
                _w.WriteAttributeString("id", id);
        }

        public void SvgEndGroup()
        {
            _w.WriteEndElement();

        }

        /// <summary>
        /// Starts an SVG "defs" element. End the group with WriteEndDefs().
        /// </summary>
        /// <param name="id">Can be null or empty or an id-string</param>
        public void SvgStartDefs(string id)
        {
            _w.WriteStartElement("defs");
            if(!String.IsNullOrEmpty(id))
                _w.WriteAttributeString("id", id);
        }
        public void SvgEndDefs()
        {
            _w.WriteEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Not written if null or empty</param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="rx"></param>
        /// <param name="ry"></param>
        /// <param name="strokeColour"></param>
        /// <param name="strokeWidth">Not written if null or empty</param>
        public void SvgEllipse(string id, float cx, float cy, float rx, float ry,
            string strokeColor, float strokeWidth, string fillColor,
            string transformString)
        {
            WriteStartElement("ellipse");
            if(!String.IsNullOrEmpty(id))
                _w.WriteAttributeString("id", id);
            WriteAttributeString("cx", cx.ToString(M.En_USNumberFormat));
            WriteAttributeString("cy", cy.ToString(M.En_USNumberFormat));
            WriteAttributeString("rx", rx.ToString(M.En_USNumberFormat));
            WriteAttributeString("ry", ry.ToString(M.En_USNumberFormat));
            if(!String.IsNullOrEmpty(strokeColor))
                WriteAttributeString("stroke", strokeColor);
            WriteAttributeString("stroke-width", strokeWidth.ToString(M.En_USNumberFormat));
            if(!String.IsNullOrEmpty(fillColor))
                _w.WriteAttributeString("fill", fillColor);
            if(!String.IsNullOrEmpty(transformString))
                WriteAttributeString("transform", transformString);
            WriteEndElement(); // ellipse
        }


        /// <summary>
        /// Writes an SVG "line" element.
        /// </summary>
        /// <param name="type">Not written if null or empty</param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="strokeColor">Not written if null or empty</param>
        /// <param name="strokeWidth">Not written if null</param>
        public void SvgLine(string type, float x1, float y1, float x2, float y2,
            string strokeColor, float strokeWidth,
            string transformString,
            string stroke_linecapString)
        {
            _w.WriteStartElement("line");
            if(!String.IsNullOrEmpty(type))
                _w.WriteAttributeString("class", type);
            _w.WriteAttributeString("x1", x1.ToString(M.En_USNumberFormat));
            _w.WriteAttributeString("y1", y1.ToString(M.En_USNumberFormat));
            _w.WriteAttributeString("x2", x2.ToString(M.En_USNumberFormat));
            _w.WriteAttributeString("y2", y2.ToString(M.En_USNumberFormat));
            if(!String.IsNullOrEmpty(strokeColor))
                _w.WriteAttributeString("stroke", strokeColor);
            _w.WriteAttributeString("stroke-width", strokeWidth.ToString(M.En_USNumberFormat));
            if(!String.IsNullOrEmpty(transformString))
                _w.WriteAttributeString("transform", transformString);
            if(!String.IsNullOrEmpty(stroke_linecapString))
                _w.WriteAttributeString("stroke-linecap", stroke_linecapString);
            _w.WriteEndElement(); //line
        }

        /// <summary>
        /// A square bracket
        /// </summary>
        public void SvgCautionaryBracket(string id, bool isLeftBracket, float top, float right, float bottom, float left, float strokeWidth)
        {
            if(!isLeftBracket)
            {
                float temp = left;
                left = right;
                right = temp;
                //temp = top;
                //top = bottom;
                //bottom = temp;
            }
            string leftStr = left.ToString(M.En_USNumberFormat);
            string topStr = top.ToString(M.En_USNumberFormat);
            string rightStr = right.ToString(M.En_USNumberFormat);
            string bottomStr = bottom.ToString(M.En_USNumberFormat);

            _w.WriteStartElement("path");
            if(!String.IsNullOrEmpty(id))
                _w.WriteAttributeString("id", id);
            _w.WriteAttributeString("stroke", "black");
            _w.WriteAttributeString("fill", "none");
            _w.WriteAttributeString("stroke-width", strokeWidth.ToString(M.En_USNumberFormat));
            _w.WriteAttributeString("stroke-linecap", "butt");
            StringBuilder d = new StringBuilder();
            d.Append("M " + rightStr + "," + topStr + " ");
            d.Append("L " + leftStr + "," + topStr + " " +
                leftStr + "," + bottomStr + " " +
                rightStr + "," + bottomStr);
            _w.WriteAttributeString("d", d.ToString());
            _w.WriteEndElement(); // path
        }
        /// <summary>
        /// Draws a vertical parallelogram with black fill and stroke.
        /// </summary>
        /// <param name="id">Not written if null or empty</param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="topLeftY"></param>
        /// <param name="topRightY"></param>
        /// <param name="beamWidth">The vertical distance between the coordinates on the left and right sides.</param>
        /// <param name="strokeWidth"></param>
        public void SvgBeam(string id, float left, float right, float topLeftY, float topRightY, float beamWidth, float strokeWidth, float opacity)
        {
            float bottomLeftY = topLeftY + beamWidth;
            float bottomRightY = topRightY + beamWidth;
            StringBuilder dSB = new StringBuilder();
            dSB.Append("M " + left.ToString(M.En_USNumberFormat) + " " + topLeftY.ToString(M.En_USNumberFormat) + " ");
            dSB.Append("L " + right.ToString(M.En_USNumberFormat) + " " + topRightY.ToString(M.En_USNumberFormat) + " ");
            dSB.Append(right.ToString(M.En_USNumberFormat) + " " + bottomRightY.ToString(M.En_USNumberFormat) + " ");
            dSB.Append(left.ToString(M.En_USNumberFormat) + " " + bottomLeftY.ToString(M.En_USNumberFormat) + " z");

            _w.WriteStartElement("path");
            if(opacity < 1.0F && opacity > 0F)
            {
                _w.WriteAttributeString("fill", "white");
                _w.WriteAttributeString("opacity", opacity.ToString(M.En_USNumberFormat));
            }
            else
            {
                _w.WriteAttributeString("stroke", "black");
                _w.WriteAttributeString("stroke-width", strokeWidth.ToString(M.En_USNumberFormat));
                _w.WriteAttributeString("fill", "black");
            }
            _w.WriteAttributeString("d", dSB.ToString());
            _w.WriteEndElement(); // path
        }

        /// <summary>
        /// Writes an SVG "rect" element.
        /// </summary>
        /// <param name="id">Can be null or empty or an id-string</param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="strokeColour">Not written if null or empty</param>
        /// <param name="strokeWidth">Not written if null</param>
        /// <param name="fillColour">Not written if null or empty</param>
        public void SvgRect(string typeString, string id, float left, float top, float width, float height,
            string strokeColour, float strokeWidth, string fillColour,
            string transformString)
        {
            Debug.Assert(strokeWidth > 0F);
            _w.WriteStartElement("rect");
            if(!String.IsNullOrEmpty(typeString))
                _w.WriteAttributeString("class", typeString);
            if(!String.IsNullOrEmpty(id))
                _w.WriteAttributeString("id", id);
            _w.WriteAttributeString("x", left.ToString(M.En_USNumberFormat));
            _w.WriteAttributeString("y", top.ToString(M.En_USNumberFormat));
            _w.WriteAttributeString("width", width.ToString(M.En_USNumberFormat));
            _w.WriteAttributeString("height", height.ToString(M.En_USNumberFormat));
            if(!String.IsNullOrEmpty(strokeColour))
                _w.WriteAttributeString("stroke", strokeColour);
            _w.WriteAttributeString("stroke-width", strokeWidth.ToString(M.En_USNumberFormat));
            if(!String.IsNullOrEmpty(fillColour))
                _w.WriteAttributeString("fill", fillColour);
            if(!String.IsNullOrEmpty(transformString))
                _w.WriteAttributeString("transform", transformString);
            _w.WriteEndElement(); // rect
        }

        public void SvgText(TextInfo textInfo, float x, float y)
        {
            _w.WriteStartElement("text");
            _w.WriteAttributeString("x", x.ToString(M.En_USNumberFormat));
            _w.WriteAttributeString("y", y.ToString(M.En_USNumberFormat));
            switch(textInfo.TextHorizAlign)
            {
                case TextHorizAlign.left:
                    break;
                case TextHorizAlign.center:
                    _w.WriteAttributeString("text-anchor", "middle");
                    break;
                case TextHorizAlign.right:
                    _w.WriteAttributeString("text-anchor", "end");
                    break;
            }
            _w.WriteAttributeString("font-size", textInfo.FontHeight.ToString(M.En_USNumberFormat));
            _w.WriteAttributeString("font-family", textInfo.FontFamily);
            if(textInfo.ColorString != null
                && !String.IsNullOrEmpty(textInfo.ColorString.String)
                && !textInfo.ColorString.IsBlack)
                _w.WriteAttributeString("fill", textInfo.ColorString.String);
            _w.WriteString(textInfo.Text);
            _w.WriteEndElement(); // text
        }

        /// <summary>
        /// Writes an SVG "use" element, overriding its y-coordinate.
        /// </summary>
        /// <param name="type">Can be null or empty or a class typeString</param>
        /// <param name="y">This element's y-coordinate.</param>
        /// <param name="objectToUse">(Do not include the leading '#'. It will be inserted automatically.)</param>
        public void SvgUseXY(string type, string objectToUse, float x, float y, float fontSize)
        {
            string transformString =
                "translate(" + x.ToString(M.En_USNumberFormat) + "," + y.ToString(M.En_USNumberFormat) + ") " +
                "scale(" + fontSize.ToString(M.En_USNumberFormat) + ")";

            _w.WriteStartElement("use");
            if(!String.IsNullOrEmpty(type))
                _w.WriteAttributeString("class", type);
            _w.WriteAttributeString("xlink", "href", null, "#" + objectToUse);
            _w.WriteAttributeString("x", "0");
            _w.WriteAttributeString("y", "0");
            _w.WriteAttributeString("transform", transformString);
            _w.WriteEndElement();
        }
        #endregion

        /// <summary>
        /// for example:
        /// [g id="Right5Flags"]
        ///     [path d="M 0,0    0,0.12096 0.31809,0.2467 Q 0.299,0.20 0.31809,0.1257" /]
        ///     [path d="M 0,0.25 0,0.37096 0.31809,0.4967 Q 0.299,0.45 0.31809,0.3757" /]
        ///     [path d="M 0,0.5  0,0.62096 0.31809,0.7467 Q 0.299,0.70 0.31809,0.6257" /]
        ///     [path d="M 0,0.75 0,0.87096 0.31809,0.9967 Q 0.299,0.95 0.31809,0.8757" /]
        ///     [path d="M 0,1    0,1.12096 0.31809,1.2467 Q 0.299,1.20 0.31809,1.1257" /]
        /// [/g]
        /// </summary>
        public void WriteFlagBlock(int nFlags, bool rightFlag)
        {
            StringBuilder type = new StringBuilder();
            if(rightFlag)
                type.Append("Right");
            else
                type.Append("Left");
            type.Append(nFlags);
            if(nFlags == 1)
                type.Append("Flag");
            else
                type.Append("Flags");

            SvgStartGroup(null, type.ToString());

            string x1 = "0";
            string x2 = "0";
            string x3 = "0.31809";
            string x4 = "0.299";
            string x5 = "0.31809";

            float sign = rightFlag ? 1F : -1F;
            float y1 = 0F;
            float y2 = sign * 0.12096F;
            float y3 = sign * 0.2467F;
            float y4 = sign * 0.2F;
            float y5 = sign * 0.1257F;
            float flagOffset = sign * 0.25F;

            for(float flagIndex = 0; flagIndex < nFlags; flagIndex++)
            {
                float offset = flagIndex * flagOffset;
                _w.WriteStartElement("path");
                StringBuilder dAttributeSB = new StringBuilder();
                dAttributeSB.Append("M ");
                dAttributeSB.Append(x1 + "," + (y1 + offset).ToString(M.En_USNumberFormat) + " ");
                dAttributeSB.Append(x2 + "," + (y2 + offset).ToString(M.En_USNumberFormat) + " ");
                dAttributeSB.Append(x3 + "," + (y3 + offset).ToString(M.En_USNumberFormat) + " Q ");
                dAttributeSB.Append(x4 + "," + (y4 + offset).ToString(M.En_USNumberFormat) + " ");
                dAttributeSB.Append(x5 + "," + (y5 + offset).ToString(M.En_USNumberFormat));
                _w.WriteAttributeString("d", dAttributeSB.ToString());
                _w.WriteEndElement();
            }
            SvgEndGroup();

        }

    }
}
