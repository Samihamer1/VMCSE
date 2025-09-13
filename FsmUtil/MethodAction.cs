/*  MethodAction, a library to add a custom FSMStateAction
    Copyright (c) 2025 SFGrenade

    There are no changes to the original file.

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
    USA*/

using System;
using HutongGames.PlayMaker;

namespace SFCore.Utils;

/// <summary>
/// FsmStateAction that invokes methods.
/// </summary>
public class MethodAction : FsmStateAction
{
    /// <summary>
    /// The method to invoke.
    /// </summary>
    public Action method;

    /// <summary>
    /// Resets the action.
    /// </summary>
    public override void Reset()
    {
        method = null;

        base.Reset();
    }

    /// <summary>
    /// Called when the action is being processed.
    /// </summary>
    public override void OnEnter()
    {
        if (method != null) method.Invoke();
        Finish();
    }
}