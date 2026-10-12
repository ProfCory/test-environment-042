// M0 test map: a 5x5 grid of city blocks ringed and split by two-lane roads.
// Pure data so it can be tested; the City scene turns it into a Phaser tilemap.

export const TILE = 32;

export const T = {
    Road: 0,
    Sidewalk: 1,
    BrickRoof: 2,
    TarRoof: 3,
    TanRoof: 4,
    Park: 5
} as const;

export const TILE_COUNT = 6;
export const SOLID_TILES: number[] = [T.BrickRoof, T.TarRoof, T.TanRoof];

export const ROAD_WIDTH = 2;          // tiles: one lane each way
export const BLOCK_SIZE = 8;          // tiles: sidewalk ring + 6x6 lot
export const PERIOD = ROAD_WIDTH + BLOCK_SIZE;
export const BLOCKS = 5;
export const MAP_TILES = PERIOD * BLOCKS + ROAD_WIDTH;

export function isRoadBand (i: number): boolean
{
    return i % PERIOD < ROAD_WIDTH;
}

// Small deterministic PRNG so the test city is the same every load.
function mulberry32 (seed: number): () => number
{
    let a = seed;
    return () =>
    {
        a = (a + 0x6d2b79f5) | 0;
        let r = Math.imul(a ^ (a >>> 15), 1 | a);
        r = (r + Math.imul(r ^ (r >>> 7), 61 | r)) ^ r;
        return ((r ^ (r >>> 14)) >>> 0) / 4294967296;
    };
}

export function buildTestMap (seed = 1978): number[][]
{
    const rand = mulberry32(seed);
    const roofs = [T.BrickRoof, T.TarRoof, T.TanRoof];
    const grid: number[][] = [];

    for (let y = 0; y < MAP_TILES; y++)
    {
        grid.push(new Array(MAP_TILES).fill(T.Road));
    }

    for (let by = 0; by < BLOCKS; by++)
    {
        for (let bx = 0; bx < BLOCKS; bx++)
        {
            const x0 = bx * PERIOD + ROAD_WIDTH;
            const y0 = by * PERIOD + ROAD_WIDTH;
            const park = bx === 1 && by === 3;

            for (let y = 0; y < BLOCK_SIZE; y++)
            {
                for (let x = 0; x < BLOCK_SIZE; x++)
                {
                    const edge = x === 0 || y === 0 || x === BLOCK_SIZE - 1 || y === BLOCK_SIZE - 1;
                    grid[y0 + y][x0 + x] = edge ? T.Sidewalk : park ? T.Park : T.Road;
                }
            }

            if (park) continue;

            // Split the 6x6 lot into a few buildings with different roofs.
            const split = 2 + Math.floor(rand() * 3);
            const lots = rand() < 0.5
                ? [[1, 1, split, 6], [1 + split, 1, 6 - split, 6]]
                : [[1, 1, 6, split], [1, 1 + split, 6, 6 - split]];

            for (const [lx, ly, w, h] of lots)
            {
                const roof = roofs[Math.floor(rand() * roofs.length)];
                for (let y = ly; y < ly + h; y++)
                {
                    for (let x = lx; x < lx + w; x++)
                    {
                        grid[y0 + y][x0 + x] = roof;
                    }
                }
            }
        }
    }

    return grid;
}

// Spawn in the eastbound (lower) lane of the top road, between the first two intersections.
export const SPAWN = {
    x: (ROAD_WIDTH + BLOCK_SIZE / 2) * TILE,
    y: (ROAD_WIDTH - 0.5) * TILE,
    heading: 0
};
