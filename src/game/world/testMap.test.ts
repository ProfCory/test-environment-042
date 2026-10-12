import { describe, expect, it } from 'vitest';
import { MAP_TILES, SOLID_TILES, SPAWN, T, TILE, buildTestMap, isRoadBand } from './testMap';

describe('buildTestMap', () =>
{
    const grid = buildTestMap();

    it('is square at the declared size', () =>
    {
        expect(grid).toHaveLength(MAP_TILES);
        for (const row of grid) expect(row).toHaveLength(MAP_TILES);
    });

    it('is deterministic for a seed', () =>
    {
        expect(buildTestMap(1978)).toEqual(grid);
    });

    it('keeps every road band fully drivable end to end', () =>
    {
        for (let i = 0; i < MAP_TILES; i++)
        {
            if (!isRoadBand(i)) continue;
            for (let j = 0; j < MAP_TILES; j++)
            {
                expect(grid[i][j]).toBe(T.Road);
                expect(grid[j][i]).toBe(T.Road);
            }
        }
    });

    it('has buildings to crash into', () =>
    {
        const solid = grid.flat().filter((t) => SOLID_TILES.includes(t)).length;
        expect(solid).toBeGreaterThan(100);
    });

    it('spawns the cab on open road', () =>
    {
        expect(grid[Math.floor(SPAWN.y / TILE)][Math.floor(SPAWN.x / TILE)]).toBe(T.Road);
    });
});
