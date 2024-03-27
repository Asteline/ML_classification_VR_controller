import csv

def add_header_and_modify(input_csv, output_csv):
    with open(input_csv, 'r') as infile, open(output_csv, 'w', newline='') as outfile:
        # Explicitly specify the delimiter (use the correct delimiter for your CSV file)
        reader = csv.reader(infile, delimiter=';')
        writer = csv.writer(outfile, delimiter=';')

        # Determine the number of columns in the input CSV
        num_columns = len(next(reader))

        max_columns = 0
        row_with_most_columns = None

        for row in reader:
            num_columns = len(row)

            if num_columns > max_columns:
                max_columns = num_columns
                row_with_most_columns = row

        infile.seek(0)
        
        # Read the header row
        header_row = next(reader)

        # Header row
        new_header_row = ["class", "cameraX"]
        for i in range(2, (abs(max_columns // 7) + 2)):
            new_header_row.extend(["velocity", "posX", "posY", "posZ", "rotX", "rotY", "rotZ"])

        # Write the header row to the output CSV
        writer.writerow(new_header_row)
        writer.writerow(header_row)

        for row in reader:
            writer.writerow(row)

if __name__ == "__main__":
    input_csv_file = "data1000_1.csv"  # Replace with the path to your input CSV file
    output_csv_file = "data1000_2.csv"  # Replace with the desired path for the output CSV file

    add_header_and_modify(input_csv_file, output_csv_file)
