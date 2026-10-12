import { GameObjects, Math as PMath, Physics, Scene, Tilemaps } from 'phaser';
import { EventBus } from '../EventBus';
import { honk } from '../audio/horn';
import { DriveControls } from '../input/DriveControls';
import { CarState, CHECKER_CAB, forwardSpeed, stepCar } from '../systems/CarModel';
import {
    BLOCKS, MAP_TILES, PERIOD, ROAD_WIDTH, SOLID_TILES, SPAWN, TILE, buildTestMap, isRoadBand
} from '../world/testMap';

// A 32px lane is ~4 m, so px/s × 0.16 reads as a believable city mph (top speed ≈ 55).
const PX_PER_S_TO_MPH = 0.16;
const HUD_INTERVAL_MS = 100;
const VIEW_TILES_TALL = 13;      // how much city fits vertically at rest
const SPEED_ZOOM_OUT = 0.15;     // zoom out this fraction at top speed
const LOOK_AHEAD = 0.35;         // seconds of travel the camera leads by
const IMPACT_SPEED = 80;         // px/s lost in one frame that counts as a crash

export class City extends Scene
{
    private cab!: Physics.Arcade.Sprite;
    private controls!: DriveControls;
    private car: CarState = { vx: 0, vy: 0, heading: SPAWN.heading };
    private baseZoom = 1;
    private hudTimer = 0;
    private lookX = 0;
    private lookY = 0;

    constructor ()
    {
        super('City');
    }

    create ()
    {
        const size = MAP_TILES * TILE;
        const map = this.make.tilemap({ data: buildTestMap(), tileWidth: TILE, tileHeight: TILE });
        const tileset = map.addTilesetImage('tiles', 'tiles', TILE, TILE, 0, 0) as Tilemaps.Tileset;
        const ground = map.createLayer(0, tileset, 0, 0) as Tilemaps.TilemapLayer;
        ground.setCollision(SOLID_TILES);

        this.drawLaneMarkings();

        this.cab = this.physics.add.sprite(SPAWN.x, SPAWN.y, 'cab');
        this.cab.setRotation(SPAWN.heading);
        // Arcade bodies don't rotate, so a centered circle stands in for the hull at any heading.
        const r = 8;
        this.cab.setCircle(r, this.cab.width / 2 - r, this.cab.height / 2 - r);
        this.cab.setBounce(0.25);
        this.cab.setCollideWorldBounds(true);
        this.cab.setDepth(10);

        this.physics.world.setBounds(0, 0, size, size);
        this.physics.add.collider(this.cab, ground);

        const cam = this.cameras.main;
        cam.setBounds(0, 0, size, size);
        cam.startFollow(this.cab, true, 0.12, 0.12);
        cam.setRoundPixels(true);
        this.fitZoom();
        this.scale.on('resize', this.fitZoom, this);

        this.controls = new DriveControls(this);

        if (new URLSearchParams(window.location.search).has('debug'))
        {
            this.physics.world.createDebugGraphic();
            (window as unknown as { __cab: Physics.Arcade.Sprite }).__cab = this.cab;
        }

        EventBus.emit('current-scene-ready', this);
    }

    update (_time: number, deltaMs: number)
    {
        const dt = Math.min(deltaMs / 1000, 0.05);
        const body = this.cab.body as Physics.Arcade.Body;
        const input = this.controls.read();

        // Pick up whatever the physics step did to us last frame (walls, bounces).
        const before = Math.hypot(this.car.vx, this.car.vy);
        this.car.vx = body.velocity.x;
        this.car.vy = body.velocity.y;
        const after = Math.hypot(this.car.vx, this.car.vy);
        if (before - after > IMPACT_SPEED) this.onImpact(before - after);

        this.car = stepCar(this.car, input, dt, CHECKER_CAB);
        body.setVelocity(this.car.vx, this.car.vy);
        this.cab.setRotation(this.car.heading);

        if (input.horn) honk();

        this.updateCamera(dt);

        this.hudTimer += deltaMs;
        if (this.hudTimer >= HUD_INTERVAL_MS)
        {
            this.hudTimer = 0;
            EventBus.emit('hud-update', {
                mph: Math.round(Math.abs(forwardSpeed(this.car)) * PX_PER_S_TO_MPH),
                reverse: forwardSpeed(this.car) < -5,
                handbrake: input.handbrake
            });
        }
    }

    private updateCamera (dt: number)
    {
        const cam = this.cameras.main;
        const speedFrac = Math.min(Math.hypot(this.car.vx, this.car.vy) / CHECKER_CAB.maxSpeed, 1);
        const ease = 1 - Math.exp(-3 * dt);

        this.lookX = PMath.Linear(this.lookX, this.car.vx * LOOK_AHEAD, ease);
        this.lookY = PMath.Linear(this.lookY, this.car.vy * LOOK_AHEAD, ease);
        // Follow offset is subtracted from the target, so lead with a negative offset.
        cam.setFollowOffset(-this.lookX, -this.lookY);
        cam.setZoom(PMath.Linear(cam.zoom, this.baseZoom * (1 - SPEED_ZOOM_OUT * speedFrac), ease));
    }

    private fitZoom ()
    {
        this.baseZoom = Math.max(1, this.scale.height / (VIEW_TILES_TALL * TILE));
        this.cameras.main.setZoom(this.baseZoom);
    }

    private onImpact (lost: number)
    {
        this.cameras.main.shake(120, Math.min(lost / 4000, 0.01));
        navigator.vibrate?.(Math.min(Math.round(lost / 4), 80));
    }

    // Dashed yellow center line on every road, broken at intersections.
    private drawLaneMarkings ()
    {
        const g: GameObjects.Graphics = this.add.graphics().setDepth(1);
        g.fillStyle(0xd9a521, 0.9);
        const dash = 12;
        const gap = 12;
        const length = MAP_TILES * TILE;

        for (let band = 0; band <= BLOCKS; band++)
        {
            const center = (band * PERIOD + ROAD_WIDTH / 2) * TILE;
            for (let p = 0; p < length; p += dash + gap)
            {
                const tile = Math.floor((p + dash / 2) / TILE);
                if (isRoadBand(tile)) continue;
                g.fillRect(p, center - 1, dash, 2);   // horizontal road
                g.fillRect(center - 1, p, 2, dash);   // vertical road
            }
        }
    }
}
