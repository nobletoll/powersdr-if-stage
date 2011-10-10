﻿//=================================================================
// FlexControlInterface1.cs
//=================================================================
// PowerSDR is a C# implementation of a Software Defined Radio.
// Copyright (C) 2004-2011  FlexRadio Systems
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// You may contact us via email at: gpl@flexradio.com.
// Paper mail may be sent to: 
//    FlexRadio Systems
//    4616 W. Howard Lane  Suite 1-150
//    Austin, TX 78728
//    USA
//=================================================================


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Flex.Control;
using System.Diagnostics;

namespace PowerSDR
{
    public enum FlexControlKnobFunction
    {
        TuneVFOA,
        TuneVFOB,
        TuneVFOASub,
        TuneRIT,
        TuneXIT,
        TuneAF,
        TuneAGCT,
        None,
    }

    public enum FlexControlButtonFunction
    {
        ToggleTXVFO, // change which VFO will transmit
        SwapVFO, // swap VFO A<>B
        ToggleSplit,
        ClearRIT,
        ClearXIT,
        CWSpeedUp,
        CWSpeedDown,
        CWXSpeedUp,
        CWXSpeedDown,
        FilterNext,
        FilterPrevious,
        SaveVFOA,
        RestoreVFOA,
        TuneStepUp,
        TuneStepDown,
        CopyVFOAtoB,
        CopyVFOBtoA,
        ZeroBeat,
        None,
    }

    class FlexControlInterface1
    {
        private static Console console;
        public static Console Console
        {
            set { console = value; }
        }

        private static object init_sync_obj = new object();
        private static bool initialized = false;
        public static bool Init()
        {
            lock (init_sync_obj)
            {
                bool b = FlexControl.Init();
                if (b)
                {
                    if (!initialized)
                    {
                        CheckVersion();
                        FlexControl.KnobRotated += new FlexControl.KnobRotatedEventHandler(FlexControl_KnobRotated);
                        FlexControl.ButtonClicked += new FlexControl.ButtonClickedEventHandler(FlexControl_ButtonClicked);
                        FlexControl.SetLEDStatus(true, true, false);
                        initialized = true;
                    }
                    else current_version = FlexControl.GetFirmwareVersion();
                }

                if (console != null && console.setupForm != null)
                    console.setupForm.FlexControlPresent = b;

                Debug.WriteLine("FlexControlInterface1.Init: " + b);
                return b;
            }
        }

        private static uint min_version = 0x00030004;

        private static uint current_version = 0;
        public static uint CurrentVersion
        {
            get { return current_version; }
        }

        private static void CheckVersion()
        {
            current_version = FlexControl.GetFirmwareVersion();

            if (current_version < min_version)
            {
                // load firmware image
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                    + "\\FLEX Firmware\\FlexControl\\";

                string file_name = path + "FlexControl_Firmware_v"+Common.RevToString(min_version)+".hex";

                bool b = FlexControl.UpdateFirmware(file_name);

                if (b)
                {
                    current_version = FlexControl.GetFirmwareVersion();
                    if (console != null && console.setupForm != null)
                        console.setupForm.FlexControlPresent = true; // force setup form to update version
                }
            }
        }

        public static void Cleanup()
        {
            try
            {
                FlexControl.SetLEDStatus(false, false, false);
            }
            catch { }
            FlexControl.Cleanup();
            if(console != null && console.setupForm != null)
                console.setupForm.FlexControlPresent = false;
        }

        private static bool auto_clear_rit_xit = false;
        public static bool AutoClearRITXIT
        {
            get { return auto_clear_rit_xit; }
            set { auto_clear_rit_xit = value; }
        }

        private static bool tuning_acceleration = true;
        public static bool TuningAcceleration
        {
            get { return tuning_acceleration; }
            set { tuning_acceleration = value; }
        }

        public enum KnobMode
        {
            A1,
            A2,
            B1,
            B2,
        }

        private static KnobMode current_knob_mode = KnobMode.A1;
        private static KnobMode CurrentKnobMode
        {
            set
            {
                FlexControlKnobFunction old_function = FlexControlKnobFunction.None;
                switch (current_knob_mode)
                {
                    case KnobMode.A1: old_function = knob_a1; break;
                    case KnobMode.A2: old_function = knob_a2; break;
                    case KnobMode.B1: old_function = knob_b1; break;
                    case KnobMode.B2: old_function = knob_b2; break;
                }

                FlexControlKnobFunction new_function = FlexControlKnobFunction.None;
                switch(value)
                {
                    case KnobMode.A1: new_function = knob_a1; break;
                    case KnobMode.A2: new_function = knob_a2; break;
                    case KnobMode.B1: new_function = knob_b1; break;
                    case KnobMode.B2: new_function = knob_b2; break;
                }

                switch (old_function)
                {
                    case FlexControlKnobFunction.TuneRIT:
                        if (new_function != FlexControlKnobFunction.TuneRIT)
                        {
                            if (console.RITOn)
                                console.RITOn = false;
                            if (auto_clear_rit_xit)
                                console.RITValue = 0;
                        }
                        break;
                    case FlexControlKnobFunction.TuneXIT:
                        if (new_function != FlexControlKnobFunction.TuneXIT)
                        {
                            if (console.XITOn)
                                console.XITOn = false;
                            if (auto_clear_rit_xit)
                                console.XITValue = 0;
                        }
                        break;
                }

                switch (new_function)
                {
                    case FlexControlKnobFunction.TuneRIT:
                        if (!console.RITOn)
                            console.RITOn = true;
                        break;
                    case FlexControlKnobFunction.TuneXIT:
                        if (!console.XITOn)
                            console.XITOn = true;
                        break;
                }

                current_knob_mode = value;
                OnCurrentKnobModeChanged(value);
            }
        }

        public delegate void KnobModeChanged(KnobMode mode);
        public static event KnobModeChanged CurrentKnobModeChanged;

        private static void OnCurrentKnobModeChanged(KnobMode mode)
        {
            if (CurrentKnobModeChanged == null) return;

            CurrentKnobModeChanged(mode);
        }

        private static FlexControlKnobFunction knob_a1 = FlexControlKnobFunction.TuneVFOA;
        public static FlexControlKnobFunction KnobA1
        {
            get { return knob_a1; }
            set { knob_a1 = value; }
        }

        private static FlexControlKnobFunction knob_a2 = FlexControlKnobFunction.TuneRIT;
        public static FlexControlKnobFunction KnobA2
        {
            get { return knob_a2; }
            set { knob_a2 = value; }
        }

        private static FlexControlKnobFunction knob_b1 = FlexControlKnobFunction.TuneVFOA;
        public static FlexControlKnobFunction KnobB1
        {
            get { return knob_b1; }
            set { knob_b1 = value; }
        }

        private static FlexControlKnobFunction knob_b2 = FlexControlKnobFunction.TuneVFOB;
        public static FlexControlKnobFunction KnobB2
        {
            get { return knob_b2; }
            set { knob_b2 = value; }
        }

        private static FlexControlButtonFunction knob_mode_a_button_double = FlexControlButtonFunction.ToggleTXVFO;
        public static FlexControlButtonFunction KnobModeAButtonDouble
        {
            get { return knob_mode_a_button_double; }
            set { knob_mode_a_button_double = value; }
        }

        private static FlexControlButtonFunction knob_mode_b_button_double = FlexControlButtonFunction.SwapVFO;
        public static FlexControlButtonFunction KnobModeBButtonDouble
        {
            get { return knob_mode_b_button_double; }
            set { knob_mode_b_button_double = value; }
        }

        private static FlexControlButtonFunction button_left_single = FlexControlButtonFunction.SwapVFO;
        public static FlexControlButtonFunction ButtonLeftSingle
        {
            get { return button_left_single; }
            set { button_left_single = value; }
        }

        private static FlexControlButtonFunction button_left_double = FlexControlButtonFunction.SwapVFO;
        public static FlexControlButtonFunction ButtonLeftDouble
        {
            get { return button_left_double; }
            set { button_left_double = value; }
        }

        private static FlexControlButtonFunction button_left_long = FlexControlButtonFunction.SwapVFO;
        public static FlexControlButtonFunction ButtonLeftLong
        {
            get { return button_left_long; }
            set { button_left_long = value; }
        }

        private static FlexControlButtonFunction button_mid_single = FlexControlButtonFunction.SwapVFO;
        public static FlexControlButtonFunction ButtonMidSingle
        {
            get { return button_mid_single; }
            set { button_mid_single = value; }
        }

        private static FlexControlButtonFunction button_mid_double = FlexControlButtonFunction.SwapVFO;
        public static FlexControlButtonFunction ButtonMidDouble
        {
            get { return button_mid_double; }
            set { button_mid_double = value; }
        }

        private static FlexControlButtonFunction button_mid_long = FlexControlButtonFunction.SwapVFO;
        public static FlexControlButtonFunction ButtonMidLong
        {
            get { return button_mid_long; }
            set { button_mid_long = value; }
        }

        private static FlexControlButtonFunction button_right_single = FlexControlButtonFunction.SwapVFO;
        public static FlexControlButtonFunction ButtonRightSingle
        {
            get { return button_right_single; }
            set { button_right_single = value; }
        }

        private static FlexControlButtonFunction button_right_double = FlexControlButtonFunction.SwapVFO;
        public static FlexControlButtonFunction ButtonRightDouble
        {
            get { return button_right_double; }
            set { button_right_double = value; }
        }

        private static FlexControlButtonFunction button_right_long = FlexControlButtonFunction.SwapVFO;
        public static FlexControlButtonFunction ButtonRightLong
        {
            get { return button_right_long; }
            set { button_right_long = value; }
        }

        /// <summary>
        /// Gets the appropraite Tune Step based on acceleration settings
        /// </summary>
        /// <param name="steps">How many input steps -- translates into bumping the 
        /// Tune Step if acceleration is turned on.</param>
        /// <returns>The Tune Step in Hz</returns>
        private static int GetTuneStep(int steps)
        {
            if (tuning_acceleration && steps > 1)
            {
                int index = console.TuneStepIndex; // get current step index
                int var_index = index + (steps = 1); // increment the step index based on accerated input
                if (var_index > console.TuneStepList.Count - 1) // cap at the top index
                    var_index = console.TuneStepList.Count - 1;

                return console.TuneStepList[var_index].StepHz;
            }
            else return console.CurrentTuneStepHz;
        }

        public static void FlexControl_KnobRotated(FlexControl.RotateDirection dir, int num_steps)
        {
            if (!initialized || console == null) return;

            FlexControlKnobFunction function = FlexControlKnobFunction.TuneVFOA;
            switch (current_knob_mode)
            {
                case KnobMode.A1: function = knob_a1; break;
                case KnobMode.A2: function = knob_a2; break;
                case KnobMode.B1: function = knob_b1; break;
                case KnobMode.B2: function = knob_b2; break;
            }

            int step = GetTuneStep(num_steps);
            if (num_steps > 1 && tuning_acceleration)
                num_steps = 1;

            switch (function)
            {
                case FlexControlKnobFunction.TuneVFOA:
                    switch (dir)
                    {
                        case FlexControl.RotateDirection.Clockwise:
                            console.VFOAFreq = console.SnapTune(console.VFOAFreq, step, num_steps);
                            break;
                        case FlexControl.RotateDirection.CounterClockwise:
                            console.VFOAFreq = console.SnapTune(console.VFOAFreq, step, -num_steps);
                            break;
                    }
                    break;
                case FlexControlKnobFunction.TuneVFOB:
                    switch (dir)
                    {
                        case FlexControl.RotateDirection.Clockwise:
                            console.VFOBFreq = console.SnapTune(console.VFOBFreq, step, num_steps);
                            break;
                        case FlexControl.RotateDirection.CounterClockwise:
                            console.VFOBFreq = console.SnapTune(console.VFOBFreq, step, -num_steps);
                            break;
                    }
                    break;
                case FlexControlKnobFunction.TuneVFOASub:
                    if (console.CurrentModel != Model.FLEX5000 || !console.RX2Enabled) return;
                    switch (dir)
                    {
                        case FlexControl.RotateDirection.Clockwise:
                            console.VFOASubFreq = console.SnapTune(console.VFOASubFreq, step, num_steps);
                            break;
                        case FlexControl.RotateDirection.CounterClockwise:
                            console.VFOASubFreq = console.SnapTune(console.VFOASubFreq, step, -num_steps);
                            break;
                    }
                    break;
                case FlexControlKnobFunction.TuneRIT:
                    if (!console.RITOn) console.RITOn = true;
                    switch (dir)
                    {
                        case FlexControl.RotateDirection.Clockwise:
                            console.RITValue += num_steps * 2;
                            break;
                        case FlexControl.RotateDirection.CounterClockwise:
                            console.RITValue -= num_steps * 2;
                            break;
                    }
                    break;
                case FlexControlKnobFunction.TuneXIT:
                    if (!console.XITOn) console.XITOn = true;
                    switch (dir)
                    {
                        case FlexControl.RotateDirection.Clockwise:
                            console.XITValue += num_steps * 2;
                            break;
                        case FlexControl.RotateDirection.CounterClockwise:
                            console.XITValue -= num_steps * 2;
                            break;
                    }
                    break;
                case FlexControlKnobFunction.TuneAF:
                    switch (dir)
                    {
                        case FlexControl.RotateDirection.Clockwise:
                            console.AF += num_steps;
                            break;
                        case FlexControl.RotateDirection.CounterClockwise:
                            console.AF -= num_steps;
                            break;
                    }
                    break;
                case FlexControlKnobFunction.TuneAGCT:
                    switch (dir)
                    {
                        case FlexControl.RotateDirection.Clockwise:
                            console.RF += num_steps;
                            break;
                        case FlexControl.RotateDirection.CounterClockwise:
                            console.RF -= num_steps;
                            break;
                    }
                    break;
                case FlexControlKnobFunction.None:
                    //do nothing
                    break;
            }
        }

        private static bool led1=true, led2=true, led3=false;

        public static void FlexControl_ButtonClicked(FlexControl.Button button, FlexControl.ClickType type)
        {
            if (!initialized || console == null) return;

            if (button == FlexControl.Button.Knob &&
                        (type == FlexControl.ClickType.Single || type == FlexControl.ClickType.Long))
            {
                switch (type)
                {
                    case FlexControl.ClickType.Single:
                        switch (current_knob_mode)
                        {
                            case KnobMode.A1: CurrentKnobMode = KnobMode.A2; led2 = false; break;
                            case KnobMode.A2: CurrentKnobMode = KnobMode.A1; led2 = true; break;
                            case KnobMode.B1: CurrentKnobMode = KnobMode.B2; led2 = false; break;
                            case KnobMode.B2: CurrentKnobMode = KnobMode.B1; led2 = true; break;
                        }
                        break;
                    case FlexControl.ClickType.Long:
                        switch (current_knob_mode)
                        {
                            case KnobMode.A1: CurrentKnobMode = KnobMode.B1; led1 = false; break;
                            case KnobMode.A2: CurrentKnobMode = KnobMode.B2; led1 = false; break;
                            case KnobMode.B1: CurrentKnobMode = KnobMode.A1; led1 = true; break;
                            case KnobMode.B2: CurrentKnobMode = KnobMode.A2; led1 = true; break;
                        }
                        break;

                }

                FlexControl.SetLEDStatus(led1, led2, led3);
                return;
            }

            FlexControlButtonFunction function = FlexControlButtonFunction.ClearRIT;

            switch (button)
            {                    
                case FlexControl.Button.Left:
                    switch (type)
                    {
                        case FlexControl.ClickType.Single: function = button_left_single; break;
                        case FlexControl.ClickType.Double: function = button_left_double; break;
                        case FlexControl.ClickType.Long: function = button_left_long; break;
                    }
                    break;
                case FlexControl.Button.Middle:
                    switch (type)
                    {
                        case FlexControl.ClickType.Single: function = button_mid_single; break;
                        case FlexControl.ClickType.Double: function = button_mid_double; break;
                        case FlexControl.ClickType.Long: function = button_mid_long; break;
                    }
                    break;
                case FlexControl.Button.Right:
                    switch (type)
                    {
                        case FlexControl.ClickType.Single: function = button_right_single; break;
                        case FlexControl.ClickType.Double: function = button_right_double; break;
                        case FlexControl.ClickType.Long: function = button_right_long; break;
                    }
                    break;
                case FlexControl.Button.Knob:
                    switch (type)
                    {
                        case FlexControl.ClickType.Double:
                            switch (current_knob_mode)
                            {
                                case KnobMode.A1:
                                case KnobMode.A2:
                                    function = knob_mode_a_button_double;
                                    break;
                                case KnobMode.B1:
                                case KnobMode.B2:
                                    function = knob_mode_b_button_double;
                                    break;
                            }
                            break;
                    }
                    break;
            }

            switch (function)
            {
                case FlexControlButtonFunction.ToggleTXVFO:
                    if (console.VFOATX)
                    {
                        console.VFOBTX = true; 
                        led3 = true;
                    }
                    else
                    {
                        console.VFOATX = true; 
                        led3 = false;
                    }
                    break;
                case FlexControlButtonFunction.SwapVFO:
                    console.VFOSwap(); led3 = false;
                    break;
                case FlexControlButtonFunction.ToggleSplit:
                    console.VFOSplit = !console.VFOSplit; led3 = console.VFOSplit;
                    break;
                case FlexControlButtonFunction.ClearRIT:
                    console.RITValue = 0; led3 = false;
                    break;
                case FlexControlButtonFunction.ClearXIT:
                    console.XITValue = 0; led3 = false;
                    break;
                case FlexControlButtonFunction.CWSpeedUp:
                    console.CATCWSpeed += 1; led3 = false;
                    break;
                case FlexControlButtonFunction.CWSpeedDown:
                    console.CATCWSpeed -= 1; led3 = false;
                    break;
                case FlexControlButtonFunction.CWXSpeedUp:
                    if(console.cwxForm != null)
                        console.cwxForm.WPM += 1; led3 = false;
                    break;
                case FlexControlButtonFunction.CWXSpeedDown:
                    if (console.cwxForm != null)
                    console.cwxForm.WPM -= 1; led3 = false;
                    break;
                case FlexControlButtonFunction.FilterNext:
                    console.RX1Filter += 1; led3 = false;
                    if (console.RX1Filter == Filter.NONE)
                        console.RX1Filter = Filter.F1;
                    break;
                case FlexControlButtonFunction.FilterPrevious:
                    console.RX1Filter -= 1; led3 = false; 
                    if (console.RX1Filter == Filter.FIRST)
                        console.RX1Filter = Filter.VAR2;
                    break;
                case FlexControlButtonFunction.SaveVFOA:
                    console.CATMemoryQS(); led3 = false;
                    break;
                case FlexControlButtonFunction.RestoreVFOA:
                    console.CATMemoryQR(); led3 = false;
                    break;
                case FlexControlButtonFunction.TuneStepUp:
                    console.CATTuneStepUp(); led3 = false;
                    break;
                case FlexControlButtonFunction.TuneStepDown:
                    console.CATTuneStepDown(); led3 = false;
                    break;
                case FlexControlButtonFunction.CopyVFOAtoB:
                    console.CopyVFOAtoB(); led3 = false;
                    break;
                case FlexControlButtonFunction.CopyVFOBtoA:
                    console.CopyVFOBtoA(); led3 = false;
                    break;
                case FlexControlButtonFunction.ZeroBeat:
                    console.ZeroBeat(); led3 = false;
                    break;
                case FlexControlButtonFunction.None:
                    //do nothing
                    break;
            }

            FlexControl.SetLEDStatus(led1, led2, led3);
        }
    }
}
