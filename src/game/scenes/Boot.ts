import { Scene } from 'phaser';
import { createCabTexture, createTileset } from '../world/textures';

export class Boot extends Scene
{
    constructor ()
    {
        super('Boot');
    }

    create ()
    {
        createTileset(this, 'tiles');
        createCabTexture(this, 'cab');
        this.scene.start('City');
    }
}
