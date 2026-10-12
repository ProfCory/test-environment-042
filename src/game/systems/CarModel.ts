// Arcade car model. Pure (no Phaser) so it can be unit tested and retuned per vehicle.
// Units: pixels, seconds, radians. Heading 0 points along +x.

export interface DriveInput
{
    throttle: number;   // 0..1
    brake: number;      // 0..1 (brakes when rolling forward, reverses when stopped)
    steer: number;      // -1 (left) .. 1 (right)
    handbrake: boolean;
}

export interface CarState
{
    vx: number;
    vy: number;
    heading: number;
}

export interface CarTuning
{
    maxSpeed: number;
    maxReverse: number;
    accel: number;
    reverseAccel: number;
    brakeDecel: number;
    coastDecel: number;
    handbrakeDecel: number;
    steerRate: number;        // rad/s at full lock once above fullSteerSpeed
    fullSteerSpeed: number;   // below this, steering scales down toward zero
    grip: number;             // lateral damping per second
    slideGrip: number;        // lateral damping per second with handbrake
}

// Era I checker cab: heavy, slow to get going, grips well.
export const CHECKER_CAB: CarTuning = {
    maxSpeed: 340,
    maxReverse: 90,
    accel: 170,
    reverseAccel: 120,
    brakeDecel: 480,
    coastDecel: 70,
    handbrakeDecel: 220,
    steerRate: 2.4,
    fullSteerSpeed: 110,
    grip: 9,
    slideGrip: 1.2
};

export const NO_INPUT: DriveInput = { throttle: 0, brake: 0, steer: 0, handbrake: false };

const STOPPED = 5;

function towardZero (value: number, amount: number): number
{
    if (value > 0) return Math.max(0, value - amount);
    if (value < 0) return Math.min(0, value + amount);
    return 0;
}

function clamp (value: number, min: number, max: number): number
{
    return Math.min(max, Math.max(min, value));
}

export function forwardSpeed (state: CarState): number
{
    return state.vx * Math.cos(state.heading) + state.vy * Math.sin(state.heading);
}

export function stepCar (state: CarState, input: DriveInput, dt: number, t: CarTuning = CHECKER_CAB): CarState
{
    const fwdBefore = forwardSpeed(state);

    // Steering authority scales with speed and flips in reverse.
    const steerScale = clamp(Math.abs(fwdBefore) / t.fullSteerSpeed, 0, 1) * Math.sign(fwdBefore);
    const heading = state.heading + clamp(input.steer, -1, 1) * t.steerRate * steerScale * dt;

    // Decompose velocity against the new heading: grip pulls it back in line, handbrake lets it slide.
    const fx = Math.cos(heading);
    const fy = Math.sin(heading);
    let fwd = state.vx * fx + state.vy * fy;
    let lat = state.vx * -fy + state.vy * fx;

    const throttle = clamp(input.throttle, 0, 1);
    const brake = clamp(input.brake, 0, 1);

    if (throttle > 0)
    {
        fwd = fwd < -STOPPED ? towardZero(fwd, t.brakeDecel * throttle * dt) : fwd + t.accel * throttle * dt;
    }
    if (brake > 0)
    {
        fwd = fwd > STOPPED ? towardZero(fwd, t.brakeDecel * brake * dt) : fwd - t.reverseAccel * brake * dt;
    }
    if (throttle === 0 && brake === 0)
    {
        fwd = towardZero(fwd, t.coastDecel * dt);
    }
    if (input.handbrake)
    {
        fwd = towardZero(fwd, t.handbrakeDecel * dt);
    }

    fwd = clamp(fwd, -t.maxReverse, t.maxSpeed);
    lat *= Math.exp(-(input.handbrake ? t.slideGrip : t.grip) * dt);

    return {
        vx: fx * fwd - fy * lat,
        vy: fy * fwd + fx * lat,
        heading
    };
}
