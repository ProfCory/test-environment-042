// Shared touch state. React's on-screen controls write it; the City scene reads it every frame.
// A plain object instead of events so the scene never sees a stale or missed release.

export const touchInput = {
    steer: 0,
    throttle: 0,
    brake: 0,
    handbrake: false,
    horn: false
};

export function resetTouchInput ()
{
    touchInput.steer = 0;
    touchInput.throttle = 0;
    touchInput.brake = 0;
    touchInput.handbrake = false;
    touchInput.horn = false;
}
