﻿using System.ComponentModel;
using YamuiFramework.Controls;

namespace YamuiDemoApp.Pages.Navigation {
    partial class Other {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.yamuiPanel1 = new YamuiFramework.Controls.YamuiPanel();
            this.yamuiButton1 = new YamuiFramework.Controls.YamuiButton();
            this.yamuiButton6 = new YamuiFramework.Controls.YamuiButton();
            this.yamuiPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // yamuiPanel1
            // 
            this.yamuiPanel1.AutoScroll = true;
            this.yamuiPanel1.Controls.Add(this.yamuiButton1);
            this.yamuiPanel1.Controls.Add(this.yamuiButton6);
            this.yamuiPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.yamuiPanel1.HorizontalScrollbar = true;
            this.yamuiPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.yamuiPanel1.HorizontalScrollbarSize = 10;
            this.yamuiPanel1.Location = new System.Drawing.Point(0, 0);
            this.yamuiPanel1.Name = "yamuiPanel1";
            this.yamuiPanel1.Size = new System.Drawing.Size(683, 893);
            this.yamuiPanel1.TabIndex = 0;
            this.yamuiPanel1.VerticalScrollbar = true;
            this.yamuiPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.yamuiPanel1.VerticalScrollbarSize = 10;
            // 
            // yamuiButton1
            // 
            this.yamuiButton1.Location = new System.Drawing.Point(25, 857);
            this.yamuiButton1.Name = "yamuiButton1";
            this.yamuiButton1.Size = new System.Drawing.Size(100, 23);
            this.yamuiButton1.TabIndex = 3;
            this.yamuiButton1.Text = "TaskWindow";
            // 
            // yamuiButton6
            // 
            this.yamuiButton6.Location = new System.Drawing.Point(0, 0);
            this.yamuiButton6.Name = "yamuiButton6";
            this.yamuiButton6.Size = new System.Drawing.Size(100, 23);
            this.yamuiButton6.TabIndex = 2;
            this.yamuiButton6.Text = "TaskWindow";
            this.yamuiButton6.Click += new System.EventHandler(this.yamuiButton6_Click);
            // 
            // Other
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.yamuiPanel1);
            this.Name = "Other";
            this.NoTransparentBackground = true;
            this.Size = new System.Drawing.Size(683, 893);
            this.yamuiPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private YamuiPanel yamuiPanel1;
        private YamuiButton yamuiButton6;
        private YamuiButton yamuiButton1;
    }
}
