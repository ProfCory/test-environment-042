import { PointerEvent as ReactPointerEvent, useEffect, useRef, useState } from 'react';
import { resetTouchInput, touchInput } from '../game/input/touchInput';

const JOYSTICK_RADIUS = 56;

// Touch UI shows on coarse-pointer devices, or anywhere with ?touch in the URL (for testing on desktop).
export function useTouchMode (): boolean
{
    const query = '(pointer: coarse)';
    const forced = new URLSearchParams(window.location.search).has('touch');
    const [coarse, setCoarse] = useState(() => window.matchMedia(query).matches);

    useEffect(() =>
    {
        const mq = window.matchMedia(query);
        const onChange = () => setCoarse(mq.matches);
        mq.addEventListener('change', onChange);
        return () => mq.removeEventListener('change', onChange);
    }, []);

    return forced || coarse;
}

// Keep receiving move/up for this finger even if it slides off the control.
function capture (e: ReactPointerEvent<HTMLElement>)
{
    try
    {
        e.currentTarget.setPointerCapture(e.pointerId);
    }
    catch
    {
        // Pointer already gone (or synthetic); the control still works without capture.
    }
}

// Floating joystick: wherever the left thumb lands becomes the center. Only X is used (steering).
function Joystick ()
{
    const [origin, setOrigin] = useState<{ x: number; y: number } | null>(null);
    const [knob, setKnob] = useState({ dx: 0, dy: 0 });
    const pointerId = useRef<number | null>(null);

    const release = () =>
    {
        pointerId.current = null;
        setOrigin(null);
        setKnob({ dx: 0, dy: 0 });
        touchInput.steer = 0;
    };

    const onDown = (e: ReactPointerEvent<HTMLDivElement>) =>
    {
        if (pointerId.current !== null) return;
        pointerId.current = e.pointerId;
        capture(e);
        setOrigin({ x: e.clientX, y: e.clientY });
    };

    const onMove = (e: ReactPointerEvent<HTMLDivElement>) =>
    {
        if (e.pointerId !== pointerId.current || !origin) return;
        let dx = e.clientX - origin.x;
        let dy = e.clientY - origin.y;
        const len = Math.hypot(dx, dy);
        if (len > JOYSTICK_RADIUS)
        {
            dx = (dx / len) * JOYSTICK_RADIUS;
            dy = (dy / len) * JOYSTICK_RADIUS;
        }
        setKnob({ dx, dy });
        touchInput.steer = dx / JOYSTICK_RADIUS;
    };

    const onUp = (e: ReactPointerEvent<HTMLDivElement>) =>
    {
        if (e.pointerId === pointerId.current) release();
    };

    return (
        <div
            className="touch-stick-zone"
            data-testid="stick-zone"
            onPointerDown={onDown}
            onPointerMove={onMove}
            onPointerUp={onUp}
            onPointerCancel={onUp}
        >
            {origin && (
                <div className="touch-stick-base" style={{ left: origin.x, top: origin.y }}>
                    <div className="touch-stick-knob" style={{ transform: `translate(${knob.dx}px, ${knob.dy}px)` }} />
                </div>
            )}
        </div>
    );
}

type HoldKey = 'throttle' | 'brake' | 'handbrake' | 'horn';

function HoldButton ({ input, label, className }: { input: HoldKey; label: string; className: string })
{
    const [held, setHeld] = useState(false);

    const set = (on: boolean) =>
    {
        setHeld(on);
        if (input === 'handbrake' || input === 'horn') touchInput[input] = on;
        else touchInput[input] = on ? 1 : 0;
        if (on) navigator.vibrate?.(8);
    };

    return (
        <button
            type="button"
            className={`touch-btn ${className} ${held ? 'held' : ''}`}
            data-testid={`btn-${input}`}
            onPointerDown={(e) => { capture(e); set(true); }}
            onPointerUp={() => set(false)}
            onPointerCancel={() => set(false)}
            onContextMenu={(e) => e.preventDefault()}
        >
            {label}
        </button>
    );
}

export function TouchControls ()
{
    // Drop any held input if the controls unmount or the tab loses focus mid-press.
    useEffect(() =>
    {
        window.addEventListener('blur', resetTouchInput);
        return () =>
        {
            window.removeEventListener('blur', resetTouchInput);
            resetTouchInput();
        };
    }, []);

    return (
        <div className="touch-layer">
            <Joystick />
            <div className="touch-right">
                <div className="touch-small">
                    <HoldButton input="horn" label="HORN" className="touch-horn" />
                    <HoldButton input="handbrake" label="HB" className="touch-hb" />
                </div>
                <div className="touch-pedals">
                    <HoldButton input="brake" label="BRAKE" className="touch-brake" />
                    <HoldButton input="throttle" label="GAS" className="touch-gas" />
                </div>
            </div>
        </div>
    );
}
