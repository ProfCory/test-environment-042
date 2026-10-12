import { AUTO, Game, Scale } from 'phaser';
import { Boot } from './scenes/Boot';
import { City } from './scenes/City';

//  Find out more information about the Game Config at:
//  https://docs.phaser.io/api-documentation/typedef/types-core#gameconfig
const config: Phaser.Types.Core.GameConfig = {
    type: AUTO,
    parent: 'game-container',
    backgroundColor: '#1a1410',
    pixelArt: true,
    scale: {
        mode: Scale.RESIZE,
        width: '100%',
        height: '100%'
    },
    physics: {
        default: 'arcade',
        arcade: { gravity: { x: 0, y: 0 } }
    },
    input: {
        gamepad: true,
        activePointers: 4
    },
    scene: [
        Boot,
        City
    ]
};

const StartGame = (parent: string) => {

    return new Game({ ...config, parent });

}

export default StartGame;
