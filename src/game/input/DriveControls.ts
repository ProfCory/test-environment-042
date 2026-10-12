// Merges keyboard, gamepad and on-screen touch into one DriveInput each frame.

import { Input, Scene } from 'phaser';
import { DriveInput } from '../systems/CarModel';
import { touchInput } from './touchInput';

type Keys = Record<'up' | 'down' | 'left' | 'right' | 'w' | 'a' | 's' | 'd' | 'space' | 'h', Input.Keyboard.Key>;

const PAD_DEADZONE = 0.15;

// Pick whichever device is pushing hardest, so switching devices mid-drive just works.
function strongest (...values: number[]): number
{
    return values.reduce((best, v) => (Math.abs(v) > Math.abs(best) ? v : best), 0);
}

export class DriveControls
{
    private keys: Keys | null = null;
    private scene: Scene;

    constructor (scene: Scene)
    {
        this.scene = scene;
        const kb = scene.input.keyboard;
        if (kb)
        {
            const K = Input.Keyboard.KeyCodes;
            this.keys = kb.addKeys({
                up: K.UP, down: K.DOWN, left: K.LEFT, right: K.RIGHT,
                w: K.W, a: K.A, s: K.S, d: K.D,
                space: K.SPACE, h: K.H
            }) as Keys;
        }
    }

    read (): DriveInput & { horn: boolean }
    {
        const k = this.keys;
        const kbThrottle = k && (k.w.isDown || k.up.isDown) ? 1 : 0;
        const kbBrake = k && (k.s.isDown || k.down.isDown) ? 1 : 0;
        const kbSteer = (k && (k.d.isDown || k.right.isDown) ? 1 : 0) - (k && (k.a.isDown || k.left.isDown) ? 1 : 0);

        // Gamepad: left stick steers, RT/LT throttle/brake, RB handbrake, B horn.
        const pad = this.scene.input.gamepad?.pad1;
        let padSteer = 0, padThrottle = 0, padBrake = 0, padHandbrake = false, padHorn = false;
        if (pad && pad.connected)
        {
            const x = pad.leftStick.x;
            padSteer = Math.abs(x) > PAD_DEADZONE ? x : 0;
            padThrottle = pad.R2;
            padBrake = pad.L2;
            padHandbrake = pad.R1 > 0.5;
            padHorn = pad.B;
        }

        return {
            throttle: Math.max(kbThrottle, padThrottle, touchInput.throttle),
            brake: Math.max(kbBrake, padBrake, touchInput.brake),
            steer: strongest(kbSteer, padSteer, touchInput.steer),
            handbrake: !!k?.space.isDown || padHandbrake || touchInput.handbrake,
            horn: !!k?.h.isDown || padHorn || touchInput.horn
        };
    }
}
