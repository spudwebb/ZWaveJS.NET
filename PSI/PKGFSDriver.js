const { fs: nodeFs } = require('@zwave-js/core/bindings/fs/node');
const path = require('path');

const CONFIG_PATH = path.resolve(__filename, '/config');
const CONFIG_PATH_IN_PKG = path.join(__dirname, `node_modules/@zwave-js/config/config`);

class PKGFSDriver {
	readFile(filePath) {
		filePath = path.normalize(filePath);
		if (filePath.startsWith(CONFIG_PATH)) {
			filePath = filePath.replace(CONFIG_PATH, CONFIG_PATH_IN_PKG);
		}
		return nodeFs.readFile(filePath);
	}

	writeFile(filePath, data) {
		filePath = path.normalize(filePath);
		if (filePath.startsWith(CONFIG_PATH)) {
			// The pkg assets are readonly
			return;
		}
		return nodeFs.writeFile(filePath, data);
	}

	copyFile(source, dest) {
		source = path.normalize(source);
		dest = path.normalize(dest);
		if (dest.startsWith(CONFIG_PATH)) {
			// The pkg assets are readonly
			return;
		}
		if (source.startsWith(CONFIG_PATH)) {
			source = source.replace(CONFIG_PATH, CONFIG_PATH_IN_PKG);
		}
		return nodeFs.copyFile(source, dest);
	}

	open(filePath, flags) {
		filePath = path.normalize(filePath);
		if (filePath.startsWith(CONFIG_PATH) && flags.write) {
			// The pkg assets are readonly
			throw new Error(`${filePath} is not writable`);
		}
		if (filePath.startsWith(CONFIG_PATH)) {
			filePath = filePath.replace(CONFIG_PATH, CONFIG_PATH_IN_PKG);
		}
		return nodeFs.open(filePath, flags);
	}

	readDir(dirPath) {
		dirPath = path.normalize(dirPath);
		if (dirPath.startsWith(CONFIG_PATH)) {
			dirPath = dirPath.replace(CONFIG_PATH, CONFIG_PATH_IN_PKG);
		}
		return nodeFs.readDir(dirPath);
	}

	stat(filePath) {
		filePath = path.normalize(filePath);
		if (filePath.startsWith(CONFIG_PATH)) {
			filePath = filePath.replace(CONFIG_PATH, CONFIG_PATH_IN_PKG);
		}
		return nodeFs.stat(filePath);
	}

	ensureDir(dirPath) {
		dirPath = path.normalize(dirPath);
		if (dirPath.startsWith(CONFIG_PATH)) {
			// The pkg assets are readonly
			return;
		}
		return nodeFs.ensureDir(dirPath);
	}

	deleteDir(dirPath) {
		dirPath = path.normalize(dirPath);
		if (dirPath.startsWith(CONFIG_PATH)) {
			// The pkg assets are readonly
			return;
		}
		return nodeFs.deleteDir(dirPath);
	}

	makeTempDir(prefix) {
		return nodeFs.makeTempDir(prefix);
	}
}

module.exports = { PKGFSDriver };
