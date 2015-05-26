# SavePics
	
Sorts picture files from an input directory into an output directory, using a folder structure with the pattern YYYY/YYYY.MM SUFFIX.

## Installation

Install with [Scoop](http://scoop.sh) from my personal [Scoop Bucket](https://github.com/masonm12/scoop-personal):

	scoop install savepics
	
Or directly:

	scoop install https://raw.githubusercontent.com/masonm12/ScoopBucket/master/savepics.json

## Usage
	
	savepics [-h] [--version] [-n] [-s [SUFFIX]] [-v] input output
	
	positional arguments:
	  input                 directory to read and sort
	  output                directory to write sorted pictures to
	
	optional arguments:
	  -h, --help            show this help message and exit
	  --version             show program's version number and exit
	  -n, --dry-run         prints out what would be written without actually
	                        writing
	  -s [SUFFIX], --suffix [SUFFIX]
	                        suffix to insert after folder name, defaults to
	                        Pictures, can be overriden to nothing
	  -v, --verbose
