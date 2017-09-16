using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CSV_Analyzer_Pro {
    public partial class CustomDgv : DataGridView {

        private const int CAPTIONHEIGHT = 21;
        private const int BORDERWIDTH = 2;

        public CustomDgv() {
            //InitializeComponent();

            // make scrollbar visible & add handler
            VerticalScrollBar.Visible = true;
            HorizontalScrollBar.Visible = true;

            VerticalScrollBar.VisibleChanged += new EventHandler(ShowScrollBars);
            HorizontalScrollBar.VisibleChanged += new EventHandler(ShowScrollBars);
        }

        private void ShowScrollBars(object sender, EventArgs e) {
            if (!VerticalScrollBar.Visible || !HorizontalScrollBar.Visible) {
                int width = VerticalScrollBar.Width;

                VerticalScrollBar.Location = new Point(ClientRectangle.Width - width - BORDERWIDTH, CAPTIONHEIGHT);

                VerticalScrollBar.Size = new Size(width, ClientRectangle.Height - CAPTIONHEIGHT - BORDERWIDTH);

                VerticalScrollBar.Show();

                //int hHeight = HorizontalScrollBar.Height;
                //int hWidth = HorizontalScrollBar.Width;

                //HorizontalScrollBar.Location = new Point(0, ClientRectangle.Height - hWidth);

               // HorizontalScrollBar.Size = new Size(hWidth,hHeight);

                //HorizontalScrollBar.Show();
            }
        }
    }
}
