using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
namespace Squareosity
{
    class ChainLink : PolygonShape
    {
        int health;
        bool isBroken = false;
        public ChainLink(int health): base(PolygonTools.CreateRectangle(0.125f, 0.125f),20)
        {
            this.health = health;



        }

        public void Update()
        {

            if (this.health <= 0)
            {
                isBroken = true;

            }
        }

        public void Draw()
        {


        }
        
        public bool getIsBroken
        {
            get { return isBroken; }

        }
    }
}
