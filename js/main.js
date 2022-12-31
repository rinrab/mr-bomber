let canvas;
let ctx;

let images;

let time = 0;

let bg;
let sprite;
let banana;
let tree;
let penguin;

const spriteWidth = 24;
const spriteHeight = 24;

const mapNeige = [
    "###################",
    "#..-------------..#",
    "#.#-#.#-#-#-#-#-#.#",
    "#---..------..----#",
    "#-#-#-###-#-#.#.#-#",
    "#-----###-----...-#",
    "#-#-#-###-#-#-#.#-#",
    "#---------..------#",
    "#-#-#-#.#-#.#-#-#-#",
    "#-----..----------#",
    "#.#-#-#-#-#-#-#.###",
    "#..-----------..###",
    "###################"
]
const mapWidth = mapNeige[0].length;
const mapHeight = mapNeige.length;

let gridImage;

const FPS = 30;

addEventListener("load", function () {
    canvas = document.getElementById("grafic");
    ctx = canvas.getContext("2d");

    bg = new AnimatedImage([
        { id: "NEIGE1", rect: new Rect(0, 0, 320, 200) },
        { id: "NEIGE2", rect: new Rect(0, 0, 320, 200) },
        { id: "NEIGE3", rect: new Rect(0, 0, 320, 200) },
    ], 1000 / FPS * 8);

    banana = new AnimatedImage(
        [
            { id: "SPRITE2", rect: new Rect(0 * 16, 0 * 16, 16, 16) },
            { id: "SPRITE2", rect: new Rect(1 * 16, 0 * 16, 16, 16) },
            { id: "SPRITE2", rect: new Rect(2 * 16, 0 * 16, 16, 16) },
            { id: "SPRITE2", rect: new Rect(3 * 16, 0 * 16, 16, 16) },
            { id: "SPRITE2", rect: new Rect(4 * 16, 0 * 16, 16, 16) },
            { id: "SPRITE2", rect: new Rect(5 * 16, 0 * 16, 16, 16) },
            { id: "SPRITE2", rect: new Rect(6 * 16, 0 * 16, 16, 16) },
            { id: "SPRITE2", rect: new Rect(7 * 16, 0 * 16, 16, 16) },
            { id: "SPRITE2", rect: new Rect(8 * 16, 0 * 16, 16, 16) },
            { id: "SPRITE2", rect: new Rect(9 * 16, 0 * 16, 16, 16) },
        ], 1000 / FPS * 5);

    penguin = new Penguin();

    tree = new AnimatedImage([
        { id: "MED3", rect: new Rect(0, 17 * 8, 32, 49) },
        { id: "MED3", rect: new Rect(33, 17 * 8, 32, 49) },
    ], 1000 / FPS * 15)

    gridImage = new AnimatedImage([
        { id: "PAUSE", rect: new Rect(0 * 16, 80, 16, 16) },
        { id: "PAUSE", rect: new Rect(1 * 16, 80, 16, 16) },
        { id: "PAUSE", rect: new Rect(2 * 16, 80, 16, 16) },
        { id: "PAUSE", rect: new Rect(3 * 16, 80, 16, 16) },
        { id: "PAUSE", rect: new Rect(4 * 16, 80, 16, 16) },
        { id: "PAUSE", rect: new Rect(5 * 16, 80, 16, 16) },
        { id: "PAUSE", rect: new Rect(6 * 16, 80, 16, 16) },
        { id: "PAUSE", rect: new Rect(7 * 16, 80, 16, 16) },
    ], -1);

    sprite = new Sprite(1);

    setInterval(timerTick, 1000 / FPS);

    addEventListener("resize", resize);
    resize();

    addEventListener("keydown", function (e) {
        if (e.code == "KeyW") {
            sprite.key = 3;
        }
        else if (e.code == "KeyS") {
            sprite.key = 0;
        }
        else if (e.code == "KeyA") {
            sprite.key = 2;
        }
        else if (e.code == "KeyD") {
            sprite.key = 1;
        } else {
            sprite.key = -1;
        }
    })

    addEventListener("keyup", function (e) {
        switch (e.code) {
            case "KeyW": case "KeyS":
            case "KeyA": case "KeyD":
                sprite.key = -1;
        }
    })
});

function resize() {
    let scale;
    if (window.innerHeight / 200 < window.innerWidth / 320) {
        scale = window.innerHeight / 200;
    } else {
        scale = window.innerWidth / 320;
    }

    canvas.style.scale = scale;
    canvas.style.left = formatCssPx((window.innerWidth - 320 * scale) / 2);
    canvas.style.top = formatCssPx((window.innerHeight - 200 * scale) / 2);
}

function formatCssPx(val) {
    return (val.toFixed(3)) + "px";
}

function timerTick() {
    time += 1000 / FPS;
    sprite.move();
    drawAll();
}

function drawAll() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    bg.draw(ctx);

    penguin.draw(ctx, 17 * 1 - 8, 0, true);
    penguin.draw(ctx, 17 * 2 - 8, 0);
    penguin.draw(ctx, 17 * 3 - 8, 0);
    penguin.draw(ctx, 17 * 7 - 8, 0);
    penguin.draw(ctx, 17 * 10 - 8, 0);
    penguin.draw(ctx, 17 * 12 - 8, 0);
    penguin.draw(ctx, 17 * 15 - 8, 0);


    for (let y in mapNeige) {
        for (let x in mapNeige[y]) {
            if (mapNeige[y][x] == "-") {
                gridImage.draw(ctx, x * 16 + 8, y * 16);
            }
        }
    }

    banana.draw(ctx, 16 * 4 + 8, 16 * 3)
    sprite.draw(ctx)


    tree.draw(ctx, 112, 30);
}

class AnimatedImage {
    images;
    currentImage;
    delay;

    _time;

    constructor(images, delay) {
        this.images = [];
        for (let img of images) {
            this.images.push({
                img: document.getElementById(img.id),
                rect: img.rect,
            });
        }

        this.delay = delay;
        this._time = 0;

        this.tick();
    }

    tick() {
        if (this.delay == -1) {
            return this.currentImage = this.images[0];
        } else {
            this._time += 1 / FPS * (1000 / this.delay);

            return this.currentImage = this.images[Math.floor(this._time) % this.images.length];
        }
    }

    draw(ctx, x = 0, y = 0, doTick = true) {
        let img = this.currentImage;
        if (doTick) {
            let img = this.tick();
        }
        ctx.drawImage(
            img.img,
            img.rect.x, img.rect.y,
            img.rect.width, img.rect.height,
            x, y,
            img.rect.width, img.rect.height);
    }
}

class Sprite {
    animations;

    x;
    y;

    animateIndex;

    key;

    speed;

    constructor(spriteIndex) {
        this.animations = [];

        this.animateIndex = 0;

        this.key = -1;

        this.speed = 2;

        let y = 1;

        const framesCount = 20;
        const framesIndex = [0, 1, 0, 2];
        for (let x = 0; x < 4; x++) {
            let newImages = [];
            for (let index of framesIndex) {
                let frameX = index + x * 3 + spriteIndex * framesCount;
                newImages.push({
                    id: "SPRITE",
                    rect: new Rect(
                        (frameX % 13) * spriteWidth, Math.floor(frameX / 13) * spriteHeight,
                        spriteWidth - 1, spriteHeight - 1)
                });
            }
            this.animations.push(new AnimatedImage(newImages, -1))
        }

        this.x = 50;
        this.y = 50;
    }

    move() {
        const delta = [
            { x: 0, y: 1 },
            { x: 1, y: 0 },
            { x: -1, y: 0 },
            { x: 0, y: -1 },
        ]

        if (this.key == -1) {
            this.animations[this.animateIndex].delay = -1;
        } else {
            this.animateIndex = this.key;
            this.animations[this.animateIndex].delay = 1000 / FPS * 7;
            this.x += delta[this.key].x * this.speed;
            this.y += delta[this.key].y * this.speed;
        }
    }

    tick() {
        this.animations[this.animateIndex].tick();
    }

    draw(ctx) {
        this.animations[this.animateIndex].draw(ctx, this.x, this.y);
    }
}

class Penguin {
    animation;

    constructor() {
        let newData = [];
        for (let i = 0; i < 5; i++) {
            newData.push(
                { id: "MED3", rect: new Rect(9 * 8 - 2, 3 * 8, 15, 15) },
                { id: "MED3", rect: new Rect(11 * 8 - 2, 3 * 8, 15, 15) });
        }
        newData.push(
            { id: "MED3", rect: new Rect(13 * 8 - 2, 3 * 8, 15, 15) },
            { id: "MED3", rect: new Rect(11 * 8 - 2, 3 * 8, 15, 15) },
            { id: "MED3", rect: new Rect(15 * 8 - 2, 3 * 8, 15, 15) },
            { id: "MED3", rect: new Rect(17 * 8 - 2, 3 * 8, 15, 15) },
            { id: "MED3", rect: new Rect(19 * 8 - 2, 3 * 8, 15, 15) },
            { id: "MED3", rect: new Rect(21 * 8 - 2, 3 * 8, 15, 15) },
            { id: "MED3", rect: new Rect(23 * 8 - 2, 3 * 8, 15, 15) },
            { id: "MED3", rect: new Rect(25 * 8 - 2, 3 * 8, 15, 15) })
        this.animation = new AnimatedImage(newData, 1000 / FPS * 7);
    }

    draw(ctx, x, y, doTick = false) {
        this.animation.draw(ctx, x, y, doTick);
    }
}

class Rect {
    x;
    y;
    width;
    height;

    constructor(x, y, width, height) {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }
}