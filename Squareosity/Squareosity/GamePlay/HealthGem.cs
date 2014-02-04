using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;

namespace Squareosity
{
    class HealthGem
    {

        Body healthGemBody;
        World world;
        int health;
        Vector2 target;
      
        int range;
        Texture2D tex;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="health"> Amount of health the gem provides</param>
        /// <param name="pos"> Starting postion of gem</param>
        /// <param name="range">The range for gem to start to seek</param>
        /// <param name="world"></param>
        public HealthGem(int health, int range,Vector2 pos,Texture2D tex,World world)
        {
            this.health = health;
            this.world = world;
            this.range = range;
            healthGemBody = BodyFactory.CreateCircle(world, 2.5f / 64, 1f, pos);

        
            this.tex = tex;
            healthGemBody.CollisionCategories = Category.Cat2;
            healthGemBody.CollidesWith = Category.All ^ Category.Cat2;
            healthGemBody.BodyType = BodyType.Dynamic;
            
           



        }
       

    }
}
