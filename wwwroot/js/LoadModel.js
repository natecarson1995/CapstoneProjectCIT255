// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
import * as THREE from "./three.js/three.module.js";
import { OrbitControls } from "./OrbitControls.js";
import { GLTFLoader } from "./GLTFLoader.js";

const loader = new GLTFLoader();
const renderer = new THREE.WebGLRenderer();
const width = window.innerWidth;
const height = window.innerHeight;
renderer.outputEncoding = THREE.sRGBEncoding;
renderer.setSize(width, height);
document.body.appendChild(renderer.domElement);

var scene = new THREE.Scene();
scene.background = new THREE.Color(255, 255, 255);

const gridHelper = new THREE.GridHelper(10,10);
scene.add(gridHelper);

var camera = new THREE.PerspectiveCamera(45, width / height, 1, 10000);

var controls = new OrbitControls(camera, renderer.domElement);

camera.position.set(5, 5, 5);
controls.update();

function animate() {
	requestAnimationFrame(animate);
	controls.update();
	renderer.render(scene, camera);
}
async function init() {
	const loadedData = await loader.loadAsync(modelURL);
	const model = loadedData.scene.children[0];
	scene.add(model);
	controls.target.copy(model.position);
	controls.maxDistance = 30;

	animate();
}
init();