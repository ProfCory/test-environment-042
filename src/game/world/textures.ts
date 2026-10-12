// Placeholder pixel art drawn at runtime so M0 ships with no binary assets.
// Era I palette: sodium orange, nicotine yellow, smog brown.

import { Scene } from 'phaser';
import { TILE, TILE_COUNT, T } from './testMap';

type Ctx = CanvasRenderingContext2D;

function speckle (ctx: Ctx, x0: number, color: string, count: number, seed: number)
{
    let s = seed;
    ctx.fillStyle = color;
    for (let i = 0; i < count; i++)
    {
        s = (s * 9301 + 49297) % 233280;
        const px = s % TILE;
        s = (s * 9301 + 49297) % 233280;
        const py = s % TILE;
        ctx.fillRect(x0 + px, py, 1, 1);
    }
}

const TILE_PAINTERS: Record<number, (ctx: Ctx, x: number) => void> = {
    [T.Road]: (ctx, x) =>
    {
        ctx.fillStyle = '#34312e';
        ctx.fillRect(x, 0, TILE, TILE);
        speckle(ctx, x, '#3e3a36', 40, 11);
        speckle(ctx, x, '#2b2826', 30, 23);
    },
    [T.Sidewalk]: (ctx, x) =>
    {
        ctx.fillStyle = '#8c8475';
        ctx.fillRect(x, 0, TILE, TILE);
        ctx.fillStyle = '#7a7366';
        ctx.fillRect(x, 15, TILE, 1);
        ctx.fillRect(x + 15, 0, 1, TILE);
        speckle(ctx, x, '#958d7e', 20, 5);
    },
    [T.BrickRoof]: (ctx, x) =>
    {
        ctx.fillStyle = '#6e3b2b';
        ctx.fillRect(x, 0, TILE, TILE);
        ctx.fillStyle = '#5e3125';
        for (let y = 3; y < TILE; y += 6) ctx.fillRect(x, y, TILE, 1);
    },
    [T.TarRoof]: (ctx, x) =>
    {
        ctx.fillStyle = '#4a4642';
        ctx.fillRect(x, 0, TILE, TILE);
        speckle(ctx, x, '#55504b', 50, 31);
        ctx.fillStyle = '#6d6a66';
        ctx.fillRect(x + 10, 10, 10, 8);
        ctx.fillStyle = '#5a5753';
        ctx.fillRect(x + 12, 12, 6, 4);
    },
    [T.TanRoof]: (ctx, x) =>
    {
        ctx.fillStyle = '#9c7f55';
        ctx.fillRect(x, 0, TILE, TILE);
        ctx.fillStyle = '#8b7049';
        ctx.fillRect(x, 0, TILE, 2);
        ctx.fillRect(x, 0, 2, TILE);
        speckle(ctx, x, '#a88b60', 25, 7);
    },
    [T.Park]: (ctx, x) =>
    {
        ctx.fillStyle = '#3f5a2c';
        ctx.fillRect(x, 0, TILE, TILE);
        speckle(ctx, x, '#4c6a35', 60, 17);
        speckle(ctx, x, '#344b24', 40, 3);
    }
};

export function createTileset (scene: Scene, key: string)
{
    const tex = scene.textures.createCanvas(key, TILE * TILE_COUNT, TILE);
    if (!tex) return;
    const ctx = tex.getContext();
    for (let i = 0; i < TILE_COUNT; i++)
    {
        TILE_PAINTERS[i](ctx, i * TILE);
    }
    tex.refresh();
}

// Checker-style cab, pointing right (+x), 30x16.
export function createCabTexture (scene: Scene, key: string)
{
    const w = 30;
    const h = 16;
    const tex = scene.textures.createCanvas(key, w, h);
    if (!tex) return;
    const ctx = tex.getContext();

    ctx.fillStyle = '#1b1712';                // shadow / tires
    ctx.fillRect(4, 0, 6, h);
    ctx.fillRect(20, 0, 6, h);

    ctx.fillStyle = '#e8b923';                // body
    ctx.fillRect(1, 1, w - 2, h - 2);

    ctx.fillStyle = '#c99a12';                // hood/trunk shading
    ctx.fillRect(1, 1, 5, h - 2);
    ctx.fillRect(w - 6, 1, 5, h - 2);

    ctx.fillStyle = '#2b3a44';                // windshield + rear glass
    ctx.fillRect(19, 3, 3, h - 6);
    ctx.fillRect(8, 3, 2, h - 6);

    ctx.fillStyle = '#f2cc4a';                // roof
    ctx.fillRect(10, 3, 9, h - 6);

    ctx.fillStyle = '#f7f1dc';                // roof light
    ctx.fillRect(13, 6, 3, 4);

    for (let i = 0; i < 6; i++)               // checker stripe along both sides
    {
        ctx.fillStyle = i % 2 ? '#111' : '#eee';
        ctx.fillRect(9 + i * 2, 1, 2, 1);
        ctx.fillRect(9 + i * 2, h - 2, 2, 1);
    }

    ctx.fillStyle = '#fff3b0';                // headlights
    ctx.fillRect(w - 2, 2, 1, 3);
    ctx.fillRect(w - 2, h - 5, 1, 3);
    ctx.fillStyle = '#b8261d';                // tail lights
    ctx.fillRect(1, 2, 1, 2);
    ctx.fillRect(1, h - 4, 1, 2);

    tex.refresh();
}
