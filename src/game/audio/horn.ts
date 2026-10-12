// Two-tone 70s car horn synthesized with Web Audio, so there's no asset to load.

let ctx: AudioContext | null = null;
let lastHonk = 0;

export function honk ()
{
    const now = performance.now();
    if (now - lastHonk < 300) return;
    lastHonk = now;

    try
    {
        ctx ??= new AudioContext();
        const t = ctx.currentTime;
        const gain = ctx.createGain();
        gain.gain.setValueAtTime(0.0001, t);
        gain.gain.exponentialRampToValueAtTime(0.12, t + 0.02);
        gain.gain.setValueAtTime(0.12, t + 0.22);
        gain.gain.exponentialRampToValueAtTime(0.0001, t + 0.3);
        gain.connect(ctx.destination);

        for (const freq of [392, 494])
        {
            const osc = ctx.createOscillator();
            osc.type = 'square';
            osc.frequency.value = freq;
            osc.connect(gain);
            osc.start(t);
            osc.stop(t + 0.3);
        }
    }
    catch
    {
        // Audio unavailable (autoplay policy, old browser): the horn is cosmetic.
    }
}
