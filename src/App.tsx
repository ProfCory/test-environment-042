import { useRef } from 'react';
import { IRefPhaserGame, PhaserGame } from './PhaserGame';
import { Hud } from './ui/Hud';
import { TouchControls, useTouchMode } from './ui/TouchControls';

function App()
{
    const phaserRef = useRef<IRefPhaserGame | null>(null);
    const touch = useTouchMode();

    return (
        <div id="app">
            <PhaserGame ref={phaserRef} />
            <Hud showKeyHints={!touch} />
            {touch && <TouchControls />}
            <div className="rotate-hint">Turn your phone sideways to drive</div>
        </div>
    )
}

export default App
