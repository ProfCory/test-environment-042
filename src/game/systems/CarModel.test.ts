import { describe, expect, it } from 'vitest';
import { CarState, CHECKER_CAB, DriveInput, NO_INPUT, forwardSpeed, stepCar } from './CarModel';

const DT = 1 / 60;

function run (state: CarState, input: Partial<DriveInput>, seconds: number): CarState
{
    let s = state;
    for (let i = 0; i < Math.round(seconds / DT); i++) s = stepCar(s, { ...NO_INPUT, ...input }, DT);
    return s;
}

const parked: CarState = { vx: 0, vy: 0, heading: 0 };

describe('stepCar', () =>
{
    it('accelerates forward along the heading', () =>
    {
        const s = run(parked, { throttle: 1 }, 1);
        expect(forwardSpeed(s)).toBeGreaterThan(100);
        expect(Math.abs(s.vy)).toBeLessThan(1e-6);
    });

    it('caps at max speed', () =>
    {
        const s = run(parked, { throttle: 1 }, 20);
        expect(forwardSpeed(s)).toBeCloseTo(CHECKER_CAB.maxSpeed, 5);
    });

    it('brakes to a stop, then reverses while brake is held', () =>
    {
        const moving = run(parked, { throttle: 1 }, 1);
        const stopped = run(moving, { brake: 1 }, 0.4);
        expect(forwardSpeed(stopped)).toBeLessThan(forwardSpeed(moving));
        const reversing = run(moving, { brake: 1 }, 3);
        expect(forwardSpeed(reversing)).toBeLessThan(0);
        expect(forwardSpeed(reversing)).toBeGreaterThanOrEqual(-CHECKER_CAB.maxReverse);
    });

    it('coasts to exactly zero without overshooting into reverse', () =>
    {
        const moving = run(parked, { throttle: 1 }, 1);
        const s = run(moving, {}, 10);
        expect(forwardSpeed(s)).toBe(0);
    });

    it('cannot turn on the spot', () =>
    {
        const s = run(parked, { steer: 1 }, 1);
        expect(s.heading).toBe(0);
    });

    it('turns right (clockwise on screen, +heading) when steering right at speed', () =>
    {
        const moving = run(parked, { throttle: 1 }, 1);
        const s = run(moving, { throttle: 1, steer: 1 }, 0.3);
        expect(s.heading).toBeGreaterThan(0);
        expect(s.vy).toBeGreaterThan(0);
    });

    it('steering is inverted in reverse, like a real car', () =>
    {
        const backing = run(parked, { brake: 1 }, 1);
        const s = run(backing, { brake: 1, steer: 1 }, 0.3);
        expect(s.heading).toBeLessThan(0);
    });

    it('handbrake keeps more sideways slide than normal grip', () =>
    {
        const moving = run(parked, { throttle: 1 }, 2);
        const lateral = (s: CarState) => Math.abs(-s.vx * Math.sin(s.heading) + s.vy * Math.cos(s.heading));
        const gripped = run(moving, { steer: 1 }, 0.3);
        const sliding = run(moving, { steer: 1, handbrake: true }, 0.3);
        expect(lateral(sliding)).toBeGreaterThan(lateral(gripped) * 2);
    });
});
