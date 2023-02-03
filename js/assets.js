function loadAssets() {
    function loadImage(imgElement, x, y, width, height) {
        return {
            img: imgElement,
            rect: new Rect(x, y, width, height),
            draw: function (ctx, x, y) {
                ctx.drawImage(
                    this.img, this.rect.x, this.rect.y, this.rect.width, this.rect.height,
                    x, y, this.rect.width, this.rect.height);
            }
        };
    }

    function loadImageStripe(imgElement, x, y, width, height, count, gap) {
        let result = [];
        count = count || 1;
        gap = gap || 0;

        for (let i = 0; i < count; i++) {
            result.push(loadImage(imgElement, x + i * (width + gap), y, width, height));
        }

        return result;
    }

    function loadPlayers(imgSpriteBoys, imgSpriteGirl) {
        let result = [];

        const framesCount = 20;
        const framesIndex = [
                [0, 1, 0, 2], [0, 1, 0, 2],
                [0, 1, 0, 2], [0, 1, 0, 2],
                [0, 1, 2, 3, 4, 5, 6, 7],
        ];

        const spriteWidth = 24;
        const spriteHeight = 24;

        const spriteIndexes = [0, 2, 3, 1];

        for (let spriteIndex of spriteIndexes) {
            let player = [];
            for (let x = 0; x < 5; x++) {
                let newImages = [];

                for (let index of framesIndex[x]) {
                    let frameX = index + x * 3 + spriteIndex * framesCount;

                    newImages.push(loadImage(imgSpriteBoys, (frameX % 13) * spriteWidth,
                        Math.floor(frameX / 13) * spriteHeight, 23, 23));
                }

                player.push(newImages);
            }

            result.push(player);
            player = [];
            for (let x = 0; x < 5; x++) {
                let newImages = [];

                for (let index of framesIndex[x]) {
                    let frameX = index + x * 3 + spriteIndex * framesCount;

                    newImages.push(loadImage(imgSpriteGirl, (frameX % 13) * spriteWidth,
                        Math.floor(frameX / 13) * (spriteHeight + 2), 23, 25));
                }

                player.push(newImages);
            }

            result.push(player);
        }

        return result;
    }

    const loadNeigeMonster = (imgSprite) => {
        const scan = [];
        for (let i = 0; i < 20; i++) {
            const x = i % 11;
            const y = Int.divFloor(i, 11) + 8;
            scan.push({
                img: imgSprite,
                rect: new Rect(x * 24, y * 18 + (y - 8), 23, 18),
                draw: function (ctx, x, y) {
                    ctx.drawImage(
                        this.img, this.rect.x, this.rect.y, this.rect.width, this.rect.height,
                        x, y, this.rect.width, this.rect.height);
                }
            })
        }
        var indexList = [
            [0, 1, 0, 2],
            [3, 4, 3, 5],
            [6, 7, 6, 8],
            [9, 10, 9, 11],
            [12, 13, 14, 15, 16, 17, 18, 19]
        ]
        const rv = [];
        for (let frames of indexList) {
            const newAnimation = [];
            rv.push(newAnimation);
            for (let frame of frames) {
                newAnimation.push(scan[frame]);
            }
        }
        return rv;
    }

    const loadMonster = (up, left, right, down, die) => {
        return [
            [up[0], up[1], up[0], up[2]],
            [left[0], left[1], left[0], left[2]],
            [right[0], right[1], right[0], right[2]],
            [down[0], down[1], down[0], down[2]],
            die
        ];
    }

    const imgNeige1 = document.getElementById("NEIGE1");
    const imgNeige2 = document.getElementById("NEIGE2");
    const imgNeige3 = document.getElementById("NEIGE3");
    const imgSprite = document.getElementById("SPRITE");
    const imgSprite2 = document.getElementById("SPRITE2");
    const imgSprite3 = document.getElementById("SPRITE3");
    const imgMed3 = document.getElementById("MED3");
    const imgPause = document.getElementById("PAUSE");
    const imgFeuille = document.getElementById("FEUILLE");
    const imgAlpha = document.getElementById("ALPHA");
    const imgGhosts = document.getElementById("GHOST");

    return {
        bomb: loadImageStripe(imgSprite2, 0 * 16, 1 * 16, 16, 16, 4),
        backGrounds:
            [
                [
                    loadImage(imgNeige1, 0, 0, 320, 200),
                    loadImage(imgNeige2, 0, 0, 320, 200),
                    loadImage(imgNeige3, 0, 0, 320, 200),
                ],
                [
                    loadImage(document.getElementById("GAME1"), 0, 0, 320, 200),
                    loadImage(document.getElementById("GAME2"), 0, 0, 320, 200),
                    loadImage(document.getElementById("GAME3"), 0, 0, 320, 200),
                ]
            ],
        niegeIgloo:
            [
                loadImage(imgMed3, 0, 77, 6 * 8, 44),
            ],
        niegeTree: loadImageStripe(imgMed3, 0, 17 * 8, 32, 49, 2, 1),
        walls: [
            loadImageStripe(imgPause, 0 * 16, 80, 16, 16, 8),
            loadImageStripe(imgPause, 0 * 16, 128, 16, 16, 8),
        ],
        neigePermanentWall: loadImage(imgPause, 272, 16, 16, 16),
        powerups:
            [
                loadImageStripe(imgSprite2, 0 * 16, 0 * 16, 16, 16, 10),
                loadImageStripe(imgSprite2, 10 * 16, 0 * 16, 16, 16, 10),
                loadImageStripe(imgSprite2, 10 * 16, 1 * 16, 16, 16, 10),
                loadImageStripe(imgSprite2, 10 * 16, 2 * 16, 16, 16, 10),
                loadImageStripe(imgSprite2, 10 * 16, 3 * 16, 16, 16, 10),
                loadImageStripe(imgSprite2, 10 * 16, 4 * 16, 16, 16, 10),
                loadImageStripe(imgSprite2, 10 * 16, 5 * 16, 16, 16, 10),
                loadImageStripe(imgSprite2, 10 * 16, 6 * 16, 16, 16, 10),
                loadImageStripe(imgSprite2, 10 * 16, 7 * 16, 16, 16, 10),
                loadImageStripe(imgSprite2, 10 * 16, 8 * 16, 16, 16, 10),
                loadImageStripe(imgSprite2, 10 * 16, 9 * 16, 16, 16, 10),
            ],
        boomMid: loadImageStripe(imgSprite2, 0 * 16, 46 + 0 * 16, 16, 16, 4),
        boomHor: loadImageStripe(imgSprite2, 0 * 16, 46 + 1 * 16, 16, 16, 4),
        boomLeftEnd: loadImageStripe(imgSprite2, 0 * 16, 46 + 2 * 16, 16, 16, 4),
        boomRightEnd: loadImageStripe(imgSprite2, 0 * 16, 46 + 3 * 16, 16, 16, 4),
        boomVert: loadImageStripe(imgSprite2, 0 * 16, 46 + 4 * 16, 16, 16, 4),
        boomTopEnd: loadImageStripe(imgSprite2, 0 * 16, 46 + 5 * 16, 16, 16, 4),
        boomBottomEnd: loadImageStripe(imgSprite2, 0 * 16, 46 + 6 * 16, 16, 16, 4),
        fire: loadImageStripe(imgSprite2, 0, 172, 26, 27, 7, 6),
        players: loadPlayers(imgSprite, imgSprite3),
        monsters: [
            loadNeigeMonster(imgSprite),
            loadMonster(
                loadImageStripe(imgMed3, 89, 56, 32, 32, 3, 1),
                loadImageStripe(imgMed3, 188, 56, 32, 32, 3, 1),
                loadImageStripe(imgMed3, 188, 89, 32, 32, 3, 1),
                loadImageStripe(imgMed3, 89, 89, 32, 32, 3, 1),
                loadImageStripe(imgMed3, 89, 122, 32, 32, 4, 1).
                    concat(loadImageStripe(imgMed3, 89, 155, 32, 32, 3, 1)))
            
        ],
        start: loadImage(document.getElementById("MENU"), 0, 0, 320, 200),
        alpha: {
            original: loadImageStripe(imgAlpha, 0, 0, 8, 6, 44),
            white: loadImageStripe(imgAlpha, 0, 8, 8, 6, 44),
            magenta: loadImageStripe(imgAlpha, 0, 16, 8, 6, 44),
            red: loadImageStripe(imgAlpha, 0, 24, 8, 6, 44),
            blue: loadImageStripe(imgAlpha, 0, 32, 8, 6, 44),
            green: loadImageStripe(imgAlpha, 0, 40, 8, 6, 44),
        },
        bigDigits: loadImageStripe(imgFeuille, 80, 83, 15, 16, 11, 1),
        draw: {
            images: [document.getElementById("DRAW1"), document.getElementById("DRAW2")],
            index: 0,
            draw: function (ctx) {
                ctx.drawImage(this.images[Math.floor(this.index) % this.images.length], 0, 0);
                this.index += 1 / 40;
            }
        },
        med: document.getElementById("MED"),
        coin: loadImageStripe(imgMed3, 0, 0, 22, 22, 13, 1)
            .concat(loadImageStripe(imgMed3, 0, 23, 22, 22, 3, 1)),
        boyGhost: loadImageStripe(imgGhosts, 0, 0, 23, 23, 12, 1),
        girlGhost: loadImageStripe(imgGhosts, 0, 24, 23, 25, 12, 1),
        vic: [
            document.getElementById("VIC1"),
            document.getElementById("VIC2"),
            document.getElementById("VIC3"),
            document.getElementById("VIC4"),
        ]

        // igloo penguin
        //    for(let i = 0; i < 5; i++) {
        //    newData.push(
        //        { id: "MED3", rect: new Rect(9 * 8 - 2, 3 * 8, 15, 15) },
        //        { id: "MED3", rect: new Rect(11 * 8 - 2, 3 * 8, 15, 15) });
        //}
        //newData.push(
        //    { id: "MED3", rect: new Rect(13 * 8 - 2, 3 * 8, 15, 15) },
        //    { id: "MED3", rect: new Rect(11 * 8 - 2, 3 * 8, 15, 15) },
        //    { id: "MED3", rect: new Rect(15 * 8 - 2, 3 * 8, 15, 15) },
        //    { id: "MED3", rect: new Rect(17 * 8 - 2, 3 * 8, 15, 15) },
        //    { id: "MED3", rect: new Rect(19 * 8 - 2, 3 * 8, 15, 15) },
        //    { id: "MED3", rect: new Rect(21 * 8 - 2, 3 * 8, 15, 15) },
        //    { id: "MED3", rect: new Rect(23 * 8 - 2, 3 * 8, 15, 15) },
        //    { id: "MED3", rect: new Rect(25 * 8 - 2, 3 * 8, 15, 15) })
    };
}
