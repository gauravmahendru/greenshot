﻿/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2012  Thomas Braun, Jens Klingen, Robin Krom
 * 
 * For more information see: http://getgreenshot.org/
 * The Greenshot project is hosted on Sourceforge: http://sourceforge.net/projects/greenshot/
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 1 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using Greenshot.Core;
using GreenshotPlugin.Core;

namespace Greenshot.Forms {
	public partial class ResizeSettingsForm : BaseForm {
		private ResizeEffect effect;
		private string value_pixel;
		private string value_percent;
		private int newWidth, newHeight;

		public ResizeSettingsForm(ResizeEffect effect) {
			this.effect = effect;
			InitializeComponent();
			this.Icon = GreenshotResources.getGreenshotIcon();
			value_pixel = Language.GetString("editor_resize_pixel");
			value_percent = Language.GetString("editor_resize_percent");
			combobox_width.Items.Add(value_pixel);
			combobox_width.Items.Add(value_percent);
			combobox_width.SelectedItem = value_pixel;
			combobox_height.Items.Add(value_pixel);
			combobox_height.Items.Add(value_percent);
			combobox_height.SelectedItem = value_pixel;
			
			textbox_width.Text = effect.Width.ToString();
			textbox_height.Text = effect.Height.ToString();
			newWidth = effect.Width;
			newHeight = effect.Height;
			combobox_width.SelectedIndexChanged += new System.EventHandler(this.combobox_SelectedIndexChanged);
			combobox_height.SelectedIndexChanged += new System.EventHandler(this.combobox_SelectedIndexChanged);

			checkbox_aspectratio.Checked = effect.MaintainAspectRatio;
		}

		private void buttonOK_Click(object sender, EventArgs e) {
			if (newWidth != effect.Width || newHeight != effect.Height) {
				effect.Width = newWidth;
				effect.Height = newHeight;
				effect.MaintainAspectRatio = checkbox_aspectratio.Checked;
				DialogResult = DialogResult.OK;
			}
		}

		private bool validate(object sender) {
			TextBox textbox = sender as TextBox;
			if (textbox != null) {
				double numberEntered;
				if (!double.TryParse(textbox.Text, out numberEntered)) {
					textbox.BackColor = Color.Red;
					return false;
				} else {
					textbox.BackColor = Color.White;
				}
			}
			return true;
		}

		private void displayWidth() {
			int displayValue;
			if (value_percent.Equals(combobox_width.SelectedItem)) {
				displayValue = (int)(((double)newWidth / (double)effect.Width) * 100);
			} else {
				displayValue = newWidth;
			}
			textbox_width.Text = displayValue.ToString();
		}
		private void displayHeight() {
			int displayValue;
			if (value_percent.Equals(combobox_height.SelectedItem)) {
				displayValue = (int)(((double)newHeight / (double)effect.Height) * 100);
			} else {
				displayValue = newHeight;
			}
			textbox_height.Text = displayValue.ToString();
		}

		private void textbox_KeyUp(object sender, KeyEventArgs e) {
			if (!validate(sender)) {
				return;
			}
			if (!checkbox_aspectratio.Checked) {
				return;
			}
			TextBox textbox = sender as TextBox;
			if (textbox.Text.Length == 0) {
				return;
			}
			bool isWidth =  textbox == textbox_width;
			bool isPercent = false;
			if (isWidth) {
				isPercent = value_percent.Equals(combobox_width.SelectedItem);
			} else {
				isPercent = value_percent.Equals(combobox_height.SelectedItem);
			}
			double percent;
			if (isWidth) {
				if (isPercent) {
					percent = float.Parse(textbox_width.Text);
					newWidth  = (int)(((double)effect.Width / 100d) * percent);
				} else {
					newWidth = int.Parse(textbox_width.Text);
					percent = ((double)double.Parse(textbox_width.Text) / (double)effect.Width) * 100d;
				}
				if (checkbox_aspectratio.Checked) {
					newHeight = (int)(((double)effect.Height / 100d) * percent);
					displayHeight();
				}
			} else {
				if (isPercent) {
					percent = int.Parse(textbox_height.Text);
					newHeight = (int)(((double)effect.Height / 100d) * percent);
				} else {
					newHeight = int.Parse(textbox_height.Text);
					percent = ((double)double.Parse(textbox_height.Text) / (double)effect.Height) * 100d;
				}
				if (checkbox_aspectratio.Checked) {
					newWidth = (int)(((double)effect.Width / 100d) * percent);
					displayWidth();
				}
			}
		}

		private void textbox_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
			validate(sender);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void combobox_SelectedIndexChanged(object sender, EventArgs e) {
			if (validate(textbox_width)) {
				displayWidth();
			}
			if (validate(textbox_height)) {
				displayHeight();
			}
		}
	}
}
