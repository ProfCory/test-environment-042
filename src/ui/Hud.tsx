import { useEffect, useState } from 'react';
import { EventBus } from '../game/EventBus';

interface HudState
{
    mph: number;
    reverse: boolean;
    handbrake: boolean;
}

export function Hud ({ showKeyHints }: { showKeyHints: boolean })
{
    const [hud, setHud] = useState<HudState>({ mph: 0, reverse: false, handbrake: false });

    useEffect(() =>
    {
        const onUpdate = (next: HudState) => setHud(next);
        EventBus.on('hud-update', onUpdate);
        return () => { EventBus.off('hud-update', onUpdate); };
    }, []);

    return (
        <>
            <div className="hud-speedo" data-testid="speedo">
                <span className="hud-speedo-value">{String(hud.mph).padStart(2, '0')}</span>
                <span className="hud-speedo-unit">MPH</span>
                <span className={`hud-gear ${hud.reverse ? 'on' : ''}`}>R</span>
                <span className={`hud-gear ${hud.handbrake ? 'on' : ''}`}>HB</span>
            </div>
            {showKeyHints && (
                <div className="hud-keys">
                    W/↑ gas · S/↓ brake/reverse · A D/← → steer · Space handbrake · H horn
                </div>
            )}
        </>
    );
}
