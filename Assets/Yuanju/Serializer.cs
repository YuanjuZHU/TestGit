using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Yuanju
{
    public class Serializer
    {
        public class list
        {
            [SerializeField]
            public List<item> componentList;

            public list() {
            }

            public list(List<item> componentList)
            {
                this.componentList = componentList;
            }

        }

        [Serializable]
        public class item
        {
            [SerializeField] public string name;

            [SerializeField] public int status;

            [SerializeField] public bool powerBlock;

            public item() {
               
            }
            public item(string name, int status, bool powerBlock)
            {
                this.name = name;
                this.status = status;
                this.powerBlock = powerBlock;
            }
        }
    }


}
