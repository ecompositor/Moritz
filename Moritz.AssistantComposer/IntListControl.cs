﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.Diagnostics;
using System.Text;

using Moritz.Globals;

namespace Moritz.AssistantComposer
{
    public partial class IntListControl : UserControl
    {
        public IntListControl(int x, int y, int minInt, int maxInt, int specialValue, Color specialValueColor, int nBoxes, ControlHasChangedDelegate controlHasChanged)
        {
            InitializeComponent();

            this.Location = new Point(x, y);
            _minInt = minInt;
            _maxInt = maxInt;

            // If a textBox's Text has this value, its ForeColor is set to specialValueColor.
            _specialValue = specialValue; 
            _specialValueColor = specialValueColor;

            _ControlHasChanged = controlHasChanged;

            Init(nBoxes);
        }

        public bool HasError()
        {
            bool rval = false;
            for(int i = 0; i < 5; ++i)
            {
                if(_boxes[i].BackColor == M.TextBoxErrorColor)
                {
                    rval = true;
                    break;
                }
            }
            return rval;
        }

        /// <summary>
        /// The attributeString contains a comma-delimited list of values.
        /// There must be the same number of values as there are boxes in this control.
        /// Note that some values can be empty (e.g. "100,,120")!
        /// </summary>
        /// <param name="attributeString"></param>
        public void Set(string attributeString)
        { 
            string[] values = attributeString.Split(',');
            Debug.Assert(values.Length == _boxes.Count);
            for(int i = 0; i < _boxes.Count; ++i)
            {
                _boxes[i].Text = values[i];
            }
        }

        /// <summary>
        /// The comma-separated list of values, that can be used as an attribute.
        /// Note that this function can return a string containing empty values (e.g. "100,,120").
        /// </summary>
        /// <returns></returns>
        public string ValuesAsString()
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < _boxes.Count; ++i)
            {
                sb.Append(_boxes[i].Text);
                sb.Append(',');
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        private void Init(int nBoxes)
        {
            int x = 0;
            for(int i = 0; i < nBoxes; ++i)
            {
                TextBox textBox = new TextBox();
                textBox.Size = new Size(30, 20);
                textBox.Location = new Point(x, 0);
                textBox.TabIndex = i;
                x += 31;
                textBox.Enter += TextBox_Enter;
                textBox.Leave += TextBox_Leave;
                _boxes.Add(textBox);
                this.Controls.Add(textBox);
            }
            this.Size = new Size(x, 20);
        }

        private void SetTextBoxState(TextBox textBox, bool okay)
        {
            if(okay)
                textBox.BackColor = Color.White;
            else
                textBox.BackColor = M.TextBoxErrorColor;
        }

        private void TextBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if(textBox != null)
            {
                textBox.Text = textBox.Text.Trim();
                M.LeaveIntRangeTextBox(textBox, true, 1, _minInt, _maxInt, SetTextBoxState);

                if(textBox.Text == _specialValue.ToString())
                {
                    textBox.ForeColor = _specialValueColor;
                }
                else
                {
                    textBox.ForeColor = Color.Black;
                }

                if(_ControlHasChanged != null)
                {
                    _ControlHasChanged(); // delegate
                }
            }
        }

        private void TextBox_Enter(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if(textBox != null)
                textBox.BackColor = Color.White;
            textBox.ForeColor = Color.Black;
        }

        public bool IsEmpty()
        {
            bool isEmpty = true;
            foreach(TextBox tb in _boxes)
            {
                if(tb.Text != "")
                {
                    isEmpty = false;
                    break;
                }
            }
            return isEmpty;
        }

        private int _minInt;
        private int _maxInt;
        private int _specialValue;
        private Color _specialValueColor;
        private ControlHasChangedDelegate _ControlHasChanged = null;
        private List<TextBox> _boxes = new List<TextBox>();
    }
}