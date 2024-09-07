using Match3.Foundation.Base.ComponentSystem;
using Match3.Game.Gameplay.Core;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Match3.Testing
{

    class ComponentA : Component
    {
        public int i = 0;
        public void Func()
        {
            i++;
        }
    }

    class ComponentB : Component
    { }

    class ComponentC : Component
    { }

    class ComponentD : Component
    { }




    public class Tester 
    {


        DateTime startTime;

        public void Test1()
        {
            startTime = DateTime.Now;
            const int iterations = 200000;
            var entity = new BasicEntity();

            entity.AddComponent(new ComponentA());
            entity.AddComponent(new ComponentB());
            entity.AddComponent(new ComponentC());
            entity.AddComponent(new ComponentD());

            for (int i = 0; i < iterations; ++i)
            {
                var c = entity.GetComponent<ComponentA>();
                //c.Func();
            }


            UnityEngine.Debug.Log(DateTime.Now.Subtract(startTime).Milliseconds);

        }

        class Key
        {
            public override int GetHashCode()
            {
                return -1;
            }
        }

        public class A
        {
            public int i;

            public A(int i)
            {
                this.i = i;
            }
        }
        public void Test2()
        {
            //A a = null;
            object obj = null;
            if(obj is A b)
            {
                
            }

            m(out A a);

            UnityEngine.Debug.Log(a.i);
        }


        void m(out A a)
        {
            a = new A(2);
        }
    }
}