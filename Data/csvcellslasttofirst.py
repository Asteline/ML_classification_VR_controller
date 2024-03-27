import csv

def move_last_to_first(input_csv, output_csv):
    with open(input_csv, 'r') as infile, open(output_csv, 'w', newline='') as outfile:
        reader = csv.reader(infile, delimiter=';')
        writer = csv.writer(outfile, delimiter=';')


        for row in reader:
            if row:
                # Find the index of the last non-empty cell
                last_non_empty_index = len(row) - 1
                while last_non_empty_index >= 0 and not row[last_non_empty_index]:
                    last_non_empty_index -= 1

                # Move the last non-empty cell to the first position
                row = [row[last_non_empty_index]] + row[:last_non_empty_index] + row[last_non_empty_index+1:]
            else:
                row = []

            # Write the modified row to the output CSV
            writer.writerow(row)

if __name__ == "__main__":
    input_csv_file = "data1000.csv"  # Replace with the path to your input CSV file
    output_csv_file = "data1000_1.csv"  # Replace with the desired path for the output CSV file

    move_last_to_first(input_csv_file, output_csv_file)
