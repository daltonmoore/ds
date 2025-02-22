﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SA
{
    public class InputHandler : MonoBehaviour
    {
        float vertical;
        float horizontal;
        bool b_input;
        bool a_input;
        bool x_input;
        bool y_input;

        bool rb_input;
        float rt_axis;
        bool rt_input;
        bool lb_input;
        float lt_axis;
        bool lt_input;

        float d_y;
        float d_x;
        bool d_up;
        bool d_down;
        bool d_right;
        bool d_left;

        bool p_d_up;//previous_d_up
        bool p_d_down;
        bool p_d_right;
        bool p_d_left;

        bool leftAxis_down;
        bool rightAxis_down;
        bool usedRightAxis;

        float b_timer;
        float rt_timer;
        float lt_timer;

        public StateManager states;
        public MyCamera camManager;

        float delta;

        void Start()
        {
            UI.QuickSlot.singleton.Init();

            states = GetComponent<StateManager>();
            states.Init();

            //camManager = CameraManager.singleton;
            //camManager.Init(states);
        }

        void FixedUpdate()
        {
            delta = Time.fixedDeltaTime;
            GetInput();
            UpdateStates();
            states.FixedTick(delta);
            //camManager.Tick(delta);
        }

        private void Update()
        {
            delta = Time.deltaTime;
            states.Tick(delta);
            ResetInputNStates();
        }

        void GetInput()
        {
            vertical = Input.GetAxis(StaticStrings.Vertical);
            horizontal = Input.GetAxis(StaticStrings.Horizontal);
            b_input = Input.GetButton(StaticStrings.B);//roll
            a_input = Input.GetButton(StaticStrings.A);//interact
            y_input = Input.GetButtonUp(StaticStrings.Y);//twohand
            x_input = Input.GetButton(StaticStrings.X);//eat item

            rt_input = Input.GetButton(StaticStrings.RT);
            rt_axis = Input.GetAxis(StaticStrings.RT);
            if (rt_axis != 0)
                rt_input = true;

            lt_input = Input.GetButton(StaticStrings.LT);
            lt_axis = Input.GetAxis(StaticStrings.LT);
            if (lt_axis != 0)
                lt_input = true;
            rb_input = Input.GetButton(StaticStrings.RB);
            lb_input = Input.GetButton(StaticStrings.LB);

            rightAxis_down = Input.GetButtonUp(StaticStrings.L) || Input.GetKeyUp(KeyCode.T);

            if (b_input)
                b_timer += delta;

            d_x = Input.GetAxis(StaticStrings.Pad_X);
            d_y = Input.GetAxis(StaticStrings.Pad_Y);

            d_up = Input.GetKeyUp(KeyCode.Alpha1) || d_y > 0;
            d_down = Input.GetKeyUp(KeyCode.Alpha2) || d_y < 0;
            d_left = Input.GetKeyUp(KeyCode.Alpha3) || d_x < 0;
            d_right = Input.GetKeyUp(KeyCode.Alpha4) || d_x > 0;
        }

        void UpdateStates()
        {
            states.horizontal = horizontal;
            states.vertical = vertical;

            Vector3 v = vertical * camManager.transform.forward;
            Vector3 h = horizontal * camManager.transform.right;
            states.moveDir = (v + h).normalized;
            float m = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            states.moveAmount = Mathf.Clamp01(m);

            if (x_input)
                b_input = false;


            if(b_input && b_timer > 0.5f)
            {
                states.run = (states.moveAmount > 0);
            }

            if (b_input == false && b_timer > 0 && b_timer < .5f)
                states.rollInput = true;

            states.itemInput = x_input;
            states.rt = rt_input;
            states.lt = lt_input;
            states.rb = rb_input;
            states.lb = lb_input;

            if(y_input)
            {
                states.isTwoHanded = !states.isTwoHanded;
                states.HandleTwoHanded();
            }
            if (states.lockOnTarget != null)
            {
                if (states.lockOnTarget.eStates != null)
                {
                    if (states.lockOnTarget.eStates.isDead)
                    {
                        states.lockOn = false;
                        states.lockOnTarget = null;
                        states.lockOnTransform = null;
                        //camManager.lockon = false;
                        //camManager.lockOnTarget = null;
                    }
                }
            }
            states.Tick(delta);
            //if (rightAxis_down)
            //{
            //    states.lockOn = !states.lockOn;

            //    states.lockOnTarget= EnemyManager.singleton.GetEnemy(transform.position);
            //    if(states.lockOnTarget == null)
            //        states.lockOn = false;

            //    camManager.lockOnTarget = states.lockOnTarget;
            //    states.lockOnTransform = states.lockOnTarget.GetTarget();
            //    camManager.lockonTransform = states.lockOnTransform;
            //    camManager.lockon = states.lockOn;
            //}

            //HandleQuickSlotChanges();
        }

        /*void HandleQuickSlotChanges()
        {
            if (states.isSpellCasting || states.usingItem)
                return;

            if (d_up)
            {
                if (!p_d_up)
                {
                    p_d_up = true;
                    states.inventoryManager.ChangeToNextSpell();
                }
            }

            if (!d_up)
            {
                p_d_up = false;
            }

            if (!d_down)
            {
                p_d_down = false;
            }

            if (states.canMove == false)
                return;

            if (d_left)
            {
                if (!p_d_left)
                {
                    states.inventoryManager.ChangeToNextWeapon(true);
                    p_d_left = true;
                }
            }
            if (d_right)
            {
                if (!p_d_right)
                {
                    states.inventoryManager.ChangeToNextWeapon(false);
                    p_d_right = true;
                }
            }

            if (!d_left)
            {
                p_d_left = false;
            }
            if (!d_right)
            {
                p_d_right = false;
            }
        }*/

        void ResetInputNStates()
        {
            if (b_input == false)
                b_timer = 0;

            if (states.rollInput)
                states.rollInput = false;

            if (states.run)
                states.run = false;
        }
    }
}
