using System.Collections.Generic;
using UnityEngine;

namespace VMCSE
{
    internal class SpritePositions : MonoBehaviour
    {

        public static Dictionary<string, Vector3[]> spritepositions = new Dictionary<string, Vector3[]>();

        private static void AddToPos(string name, double left, double right, double top, double bottom)
        {
            //assumes knight facing the right.
            List<Vector3> list = new List<Vector3>
            {
                new Vector3((float)left, (float)bottom, 0),
                new Vector3((float) right, (float) bottom, 0),
                new Vector3((float) left, (float) top, 0),
                new Vector3((float) right, (float) top, 0),
            };

            spritepositions.Add(name, list.ToArray());
        }

        private static void AddToPosBulk(string name, double left, double right, double top, double bottom, int lowerinclusive, int upperinclusive)
        {
            for (int i = lowerinclusive; i < upperinclusive + 1; i++)
            {
                AddToPos(name + i, left, right, top, bottom);
            }
        }

        public static void Initialise()
        {
            //SPOILER ALERT.
            //I could NOT figure out a better way to do it. Oh well.

            //AddToPosBulk("IdleGV", -0.4219, 0.5312, 0.625, -1.4063,1,9);



        }
    }
}
