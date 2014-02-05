using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
namespace Squareosity
{


    class ChoiceDisplay
    {
        Texture2D A, B, X, Y;
        String AText, BText, XText, YText, Subs;
        bool active = false;
        Vector2 orgin = new Vector2(15, 15);
        ContentManager content;
        Vector2 postion = new Vector2(1024 / 2, 700);

    


        SpriteFont font;

        public ChoiceDisplay(Texture2D A, Texture2D B, Texture2D X, Texture2D Y, String AText, String BText, String XText, String YText, ContentManager content )
        {
            this.A = A;
            this.B = B;
            this.X = X;
            this.Y = Y;

            this.AText = AText;
            this.BText = BText;
            this.XText = XText;
            this.YText = YText;

            this.content = content;

            font = content.Load<SpriteFont>("subsFont");

        }
        public void Draw(SpriteBatch batch)
        {
            if (active /*&& GamePad.GetState(PlayerIndex.One).IsConnected*/)
            {
               
                Vector2 AtextSize = font.MeasureString(AText);
                Vector2 AtextPos = new Vector2(postion.X - (AtextSize.X / 2f),postion.Y + 10 + AtextSize.Y) ;
                batch.Draw(A, postion, null,Color.White,0f,orgin,1f,SpriteEffects.None,1f);
                batch.DrawString(font, AText, AtextPos, Color.White);


                Vector2 BtextSize = font.MeasureString(BText);
                Vector2 BtextPos = new Vector2(postion.X + 65, postion.Y - 40 - (BtextSize.Y / 2));
                batch.Draw(B, postion + new Vector2(40,-40),null ,Color.White,0f, orgin, 1f, SpriteEffects.None, 1f);
                batch.DrawString(font, BText, BtextPos, Color.White);



                Vector2 XtextSize = font.MeasureString(XText);
                Vector2 XtextPos = new Vector2(postion.X - 65 - XtextSize.X , postion.Y - 40 - (XtextSize.Y / 2));

                batch.DrawString(font, XText, XtextPos, Color.White);

                batch.Draw(X, postion + new Vector2(-40, -40), null, Color.White, 0f, orgin, 1f, SpriteEffects.None, 1f);
            }

            if (Subs != null)
            {
                if (active)
                {
                    Vector2 SubstextSize = font.MeasureString(Subs);
                    Vector2 SubstextPos = new Vector2((postion.X) - (SubstextSize.X / 2), 600);

                    batch.DrawString(font, Subs, SubstextPos, Color.White);
                }
                else
                {
                    Vector2 SubstextSize = font.MeasureString(Subs);
                    Vector2 SubstextPos = new Vector2((postion.X) - (SubstextSize.X / 2), 720);

                    batch.DrawString(font, Subs, SubstextPos, Color.White);
                }
            }
        }
        public bool Acitve
        {
            get { return active; }
            set { active = value; }
        }
        public String setSub
        {
            set { Subs = value; }
        }
        public String setTextA
        {
            set { AText = value; }
        }
        public String setTextB
        {
            set { BText = value; }
        }

        public String setTextX
        {
            set { XText = value; }
        }

        public String setTextY
        {
            set { YText = value; }
        }
        


    }
}
