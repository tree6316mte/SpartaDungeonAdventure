using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimState
{
    IDLE,
    MOVE,
    JUMP,
    // ATTACK,
    // DAMAGED,
    // DEBUFF,
    // DEATH,
    // OTHER,
}


public class PlayerAnimHandler : MonoBehaviour
{
    private Animator _animator;

    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    public void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.updateMode = AnimatorUpdateMode.AnimatePhysics;

        AssignAnimationIDs();
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    public void PlayMove(float _moveSpeed, float _motionSpeed)
    {
        _animator.SetFloat(_animIDSpeed, _moveSpeed);
        _animator.SetFloat(_animIDMotionSpeed, _motionSpeed);
    }

    public void PlayJump(bool _jump)
    {
        _animator.SetBool(_animIDJump, _jump);
    }

    public void PlayFreeFall(bool _fall)
    {
        _animator.SetBool(_animIDFreeFall, _fall);
    }


    public void SetGrounded(bool _grounded)
    {
        _animator.SetBool(_animIDGrounded, _grounded);
    }
}
