﻿using System;
using Infrastructure.Factories;
using Infrastructure.Helpers;
using UnityEngine;

namespace Infrastructure.Services.Input
{
    public interface IInputService
    {
        Vector2 Moving();
        Observable Attacked { get; }
    }
}