using System;
using Core.Binders;
using Infrastructure.Factories;
using UnityEngine;

namespace Infrastructure.Services.Input
{
    public interface IInputService
    {
        Vector2 Moving();
        Observable Attacked { get; }
    }
}